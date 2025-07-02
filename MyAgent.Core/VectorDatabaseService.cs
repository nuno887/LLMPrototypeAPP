using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Core
{
    public class VectorDatabaseService
    {
        private readonly string _connectionString;
        private readonly EmbeddingGenerator _embeddingGen;

        public VectorDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
            _embeddingGen = new EmbeddingGenerator();
        }

        public async Task<List<string>> SearchAsync(string query, int topK = 1, string target = "Completo")
        {
            var results = new List<string>();

            string embeddingJson = await _embeddingGen.GenerateEmbeddingAsync(query);

            // Use full 768 dimensions for SQL Server VECTOR compatibility
            string[] parts = embeddingJson.Trim('[', ']').Split(',');
            string truncatedJson = "[" + string.Join(",", parts.Select(p => p.Trim())) + "]";

            string embeddingColumn = target switch
            {
                "Sumario" => "Sumario_Embedding",
                "Texto" => "Texto_Embedding",
                "Anexo" => "Anexo_Embedding",
                _ => "Completo_Embedding"
            };

            string sql = $@"
    SELECT TOP (@TopK) 
        r.RelatorioId, 
        r.RelatorioIdentifier, 
        r.Sumario,
        r.Texto,
        r.Anexo,
        r.Completo,
        b.BranchName,
        p.PdfName,
        VECTOR_DISTANCE('cosine', {embeddingColumn}, CAST(@QueryEmbedding AS VECTOR(768))) AS Distance
    FROM Relatorios r
    LEFT JOIN Branches b ON r.BranchId = b.BranchId
    LEFT JOIN PdfFiles p ON r.PdfId = p.PdfId
    ORDER BY Distance ASC;
";


            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@TopK", topK);
            cmd.Parameters.AddWithValue("@QueryEmbedding", truncatedJson);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string identifier = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                string sumario = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                string texto = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                string anexo = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                string completo = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                string branchName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                string pdfName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                double distance = reader.IsDBNull(8) ? 0.0 : reader.GetDouble(8);

                string resultText = $@"
[Relatorio: {identifier}]
[Branch: {branchName}]
[PDF: {pdfName}]
[Sumario]: {sumario}
[Texto]: {texto}
[Anexo]: {anexo}
[Completo]: {completo}
[Distance]: {distance}
";

                results.Add(resultText.Trim());
            }

            return results;

        }
    }
}