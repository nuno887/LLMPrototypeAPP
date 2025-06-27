using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MyAgent.Core
{
    public class RelatorioProcessor
    {
        private readonly string _connectionString;

        public RelatorioProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ProcessJsonFile(string jsonFilePath)
        {
            string projectRoot = @"C:\Users\nuno.ms.goncalves\Desktop\SpaCy_NET"; // Your manual path to DATA's parent folder

            var relatorios = JsonConvert.DeserializeObject<List<RelatorioData>>(File.ReadAllText(jsonFilePath));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                foreach (var relatorio in relatorios)
                {
                    int branchId = GetOrInsertBranch(conn, relatorio.secretaria);
                    int pdfId = GetOrInsertPdf(conn, relatorio.PDF);

                    // Build full folder path to the report folder
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

                    string insertRelatorio = @"
                INSERT INTO Relatorios (RelatorioIdentifier, BranchId, PdfId, Sumario, Texto, Anexo, Completo)
                VALUES (@RelatorioIdentifier, @BranchId, @PdfId, @Sumario, @Texto, @Anexo, @Completo);";

                    using (SqlCommand cmd = new SqlCommand(insertRelatorio, conn))
                    {
                        cmd.Parameters.AddWithValue("@RelatorioIdentifier", relatorio.despacho);
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@PdfId", pdfId);
                        cmd.Parameters.AddWithValue("@Sumario", (object)sumario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Texto", (object)texto ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Anexo", (object)anexo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Completo", completo);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
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
    }
}
