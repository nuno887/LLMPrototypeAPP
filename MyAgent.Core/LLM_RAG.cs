using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Core;

public class LLM_RAG
{
    private readonly string _connectionString;

    public LLM_RAG(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<LLMResult> ProcessQuestionAsync(string question)
    {
        // Vector search logic here (query SQL Server for relevant documents)
        // Build answer based on retrieved documents

        var retrievedDocs = await SearchVectorDatabaseAsync(question);

        if (retrievedDocs.Count == 0)
        {
            return new LLMResult
            {
                Notes = "[RAG] No relevant documents found.",
                Answer = "No relevant information was found to answer your question."
            };
        }

        var notes = new StringBuilder();
        notes.AppendLine("=== Retrieved Documents ===");
        foreach (var doc in retrievedDocs)
        {
            notes.AppendLine(doc);
            notes.AppendLine("---");
        }

        return new LLMResult
        {
            Notes = notes.ToString(),
            Answer = string.Join("\n", retrievedDocs)
        };
    }

    private async Task<List<string>> SearchVectorDatabaseAsync(string query)
    {
        var results = new List<string>();

        // Example: Replace with your actual vector search SQL
        string sql = @"
            DECLARE @queryVector VECTOR(768) = VECTOR::Parse(@Embedding);

            SELECT TOP 5 Completo
            FROM Relatorios
            ORDER BY Completo_Embedding.Similarity(@queryVector) DESC;
        ";

        string embeddingJson = await new EmbeddingGenerator().GenerateEmbeddingAsync(query);

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Embedding", embeddingJson);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(reader.GetString(0));
        }

        return results;
    }
}

