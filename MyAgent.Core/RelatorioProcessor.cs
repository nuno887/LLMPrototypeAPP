using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyAgent.Core
{
    public class RelatorioProcessor
    {
        private readonly string _connectionString;

        public RelatorioProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ProcessJsonFileAsync(string jsonFilePath)
        {
            string projectRoot = @"C:\Users\nuno.ms.goncalves\Desktop\SpaCy_NET";
            var embeddingGenerator = new EmbeddingGenerator();

            var relatorios = JsonConvert.DeserializeObject<List<RelatorioData>>(File.ReadAllText(jsonFilePath));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                foreach (var relatorio in relatorios)
                {
                    if (RelatorioExists(conn, relatorio.despacho))
                    {
                        Console.WriteLine($"Skipping {relatorio.despacho} - Already exists in the database.");
                        continue;
                    }

                    int branchId = GetOrInsertBranch(conn, relatorio.secretaria);
                    int pdfId = GetOrInsertPdf(conn, relatorio.PDF);

                    string fullFolderPath = Path.Combine(projectRoot, relatorio.path);

                    string sumario = ReadTextIfExists(Path.Combine(fullFolderPath, "sumario.txt"));
                    string texto = ReadTextIfExists(Path.Combine(fullFolderPath, "texto.txt"));
                    string anexo = ReadTextIfExists(Path.Combine(fullFolderPath, "anexo.txt"));
                    string completo = ReadTextIfExists(Path.Combine(fullFolderPath, "completo.txt"));

                    if (string.IsNullOrWhiteSpace(completo))
                    {
                        Console.WriteLine($"Skipping {relatorio.despacho} - No full content found.");
                        continue;
                    }

                    string sumarioEmbedding = string.IsNullOrWhiteSpace(sumario) ? null : await embeddingGenerator.GenerateEmbeddingAsync(sumario);
                    string textoEmbedding = string.IsNullOrWhiteSpace(texto) ? null : await embeddingGenerator.GenerateEmbeddingAsync(texto);
                    string anexoEmbedding = string.IsNullOrWhiteSpace(anexo) ? null : await embeddingGenerator.GenerateEmbeddingAsync(anexo);
                    string completoEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(completo);

                    if (sumarioEmbedding != null) ValidateEmbedding(sumarioEmbedding, "Sumario");
                    if (textoEmbedding != null) ValidateEmbedding(textoEmbedding, "Texto");
                    if (anexoEmbedding != null) ValidateEmbedding(anexoEmbedding, "Anexo");
                    ValidateEmbedding(completoEmbedding, "Completo");

                    string insertRelatorio = @"
                INSERT INTO Relatorios 
                (RelatorioIdentifier, BranchId, PdfId, Sumario, Texto, Anexo, Completo,
                 Sumario_Embedding, Texto_Embedding, Anexo_Embedding, Completo_Embedding)
                VALUES 
                (@RelatorioIdentifier, @BranchId, @PdfId, @Sumario, @Texto, @Anexo, @Completo,
                 @SumarioEmbedding, @TextoEmbedding, @AnexoEmbedding, @CompletoEmbedding);";

                    using (SqlCommand cmd = new SqlCommand(insertRelatorio, conn))
                    {
                        cmd.Parameters.AddWithValue("@RelatorioIdentifier", relatorio.despacho);
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@PdfId", pdfId);
                        cmd.Parameters.AddWithValue("@Sumario", (object)sumario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Texto", (object)texto ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Anexo", (object)anexo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Completo", completo);

                        cmd.Parameters.AddWithValue("@SumarioEmbedding", (object)sumarioEmbedding ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TextoEmbedding", (object)textoEmbedding ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AnexoEmbedding", (object)anexoEmbedding ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompletoEmbedding", completoEmbedding);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        private void ValidateEmbedding(string embeddingJson, string fieldName)
        {
            var vector = JsonConvert.DeserializeObject<List<float>>(embeddingJson);

            if (vector.Count != 768)
                throw new InvalidOperationException($"Embedding for {fieldName} must have exactly 768 dimensions. Found {vector.Count}.");
        }

        private string ReadTextIfExists(string path)
        {
            return (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) ? null : File.ReadAllText(path);
        }

        private int GetOrInsertBranch(SqlConnection conn, string branchName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT BranchId FROM Branches WHERE BranchName = @name", conn))
            {
                cmd.Parameters.AddWithValue("@name", branchName);
                var result = cmd.ExecuteScalar();
                if (result != null) return (int)result;
            }

            using (SqlCommand cmd = new SqlCommand("INSERT INTO Branches (BranchName) OUTPUT INSERTED.BranchId VALUES (@name)", conn))
            {
                cmd.Parameters.AddWithValue("@name", branchName);
                return (int)cmd.ExecuteScalar();
            }
        }

        private int GetOrInsertPdf(SqlConnection conn, string pdfName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT PdfId FROM PdfFiles WHERE PdfName = @name", conn))
            {
                cmd.Parameters.AddWithValue("@name", pdfName);
                var result = cmd.ExecuteScalar();
                if (result != null) return (int)result;
            }

            using (SqlCommand cmd = new SqlCommand("INSERT INTO PdfFiles (PdfName) OUTPUT INSERTED.PdfId VALUES (@name)", conn))
            {
                cmd.Parameters.AddWithValue("@name", pdfName);
                return (int)cmd.ExecuteScalar();
            }
        }

        private bool RelatorioExists(SqlConnection conn, string relatorioIdentifier)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Relatorios WHERE RelatorioIdentifier = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", relatorioIdentifier);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
