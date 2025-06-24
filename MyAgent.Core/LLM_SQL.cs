using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Text.Json;
using System.Diagnostics;

namespace MyAgent.Core;

public class LLM_SQL
{
    private readonly OllamaClient _ollama;
    private readonly string _connectionString;
    private readonly HashSet<string> AllowedTables = new();
    private const int MaxAttempts = 3;

    public LLM_SQL(string dbPath, string modelName)
    {
        _connectionString = $"Data Source={dbPath};Version=3;";
        _ollama = new OllamaClient(modelName);
    }

    public async Task<LLMResult> ProcessQuestionAsync(string question)
    {
        string schemaInfo = GetDatabaseSchema();

        string prompt = $@"
You are a helpful assistant who answers questions using SQL queries.
Use the actual SQL table and column names from the schema below.

Users may ask questions in English or Portuguese. Interpret them accordingly.

Synonyms:
- 'regists', 'entries', 'records', 'registos', 'entradas', 'registros' → despachos  
- 'authors', 'people', 'person', 'autores', 'pessoas', 'indivíduos' → pessoas  
- 'documents', 'files', 'documentos', 'ficheiros', 'arquivos' → pdf_files  
- 'date', 'data', 'datas' → datas  
- 'series', 'série', 'séries' → series  
- 'office', 'department', 'secretaria', 'departamento', 'gabinete' → secretarias

Strict Rules:
- Do NOT include explanations, comments, or markdown.
- Do NOT use triple backticks or extra formatting.
- Do NOT invent column names or table names.
- Only use fields explicitly shown in the schema.

Schema:
{schemaInfo}

Rules:
- Only use SELECT statements. Never use INSERT, UPDATE, or DELETE.
- Use JOINs with clear aliases (e.g., d for despachos, p for pessoas).
- Limit results to relevant fields; avoid SELECT *.
- Never guess names, filters, or terms not explicitly mentioned by the user.
- Use d.autor_id → p.id to find the author of a despacho.
- Use despacho_pessoas to find other mentioned people in a despacho.
- Always prioritize accuracy over guessing.
- If unsure about a term, ask the user to clarify.

Formatting:
- Return only the SQL query as output. Do not explain or add any extra text.

User question:
{question}
";

        string sqlQuery = string.Empty;
        string cleanSqlQuery = string.Empty;

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            sqlQuery = await _ollama.AskAsync(prompt);
            cleanSqlQuery = sqlQuery;

            try
            {
                var jsonDoc = JsonDocument.Parse(sqlQuery);
                if (jsonDoc.RootElement.TryGetProperty("response", out var responseElement))
                {
                    cleanSqlQuery = responseElement.GetString();
                }
            }
            catch
            {
            }

            if (IsValidSQL(cleanSqlQuery))
            {
                break;
            }

            if (attempt == MaxAttempts)
            {
                return new LLMResult
                {
                    Notes = "[Blocked] SQL contains unauthorized tables after multiple attempts.",
                    Answer = "[Blocked] SQL contains unauthorized tables after multiple attempts."
                };
            }

        }

        string queryResult = ExecuteQuery(cleanSqlQuery);

        string finalPrompt = $"User asked: '{question}'. SQL result: {queryResult}. Provide a clear, friendly response.";
        string finalResponse = await _ollama.AskAsync(finalPrompt);
        string cleanFinalResponse = finalResponse;

        try
        {
            var jsonDoc = JsonDocument.Parse(finalResponse);
            if (jsonDoc.RootElement.TryGetProperty("response", out var responseElement))
            {
                cleanFinalResponse = responseElement.GetString();
            }
        }
        catch
        {
        }

        return new LLMResult
        {
            Notes = queryResult,
            Answer = cleanFinalResponse
        };
    }

    private string GetDatabaseSchema()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var schemaText = new StringBuilder();
        AllowedTables.Clear();

        using var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';", connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string tableName = reader.GetString(0);
            AllowedTables.Add(tableName);
            schemaText.AppendLine($"\nTable: {tableName}");

            using var columnCmd = new SQLiteCommand($"PRAGMA table_info({tableName});", connection);
            using var columnReader = columnCmd.ExecuteReader();

            while (columnReader.Read())
            {
                string colName = columnReader.GetString(1);
                schemaText.AppendLine($"  - {colName}");
            }
        }

        if (schemaText.Length == 0)
        {
            schemaText.AppendLine("No tables found in database.");
        }

        return schemaText.ToString();
    }

    private bool IsValidSQL(string sql)
    {
        foreach (string table in AllowedTables)
        {
            if (sql.Contains(table, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private string ExecuteQuery(string sql)
    {
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var cmd = new SQLiteCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            var resultText = new StringBuilder();

            while (reader.Read())
            {
                string value = reader.GetValue(0)?.ToString() ?? string.Empty;
                resultText.AppendLine(value);
            }

            return resultText.Length > 0 ? resultText.ToString() : "No result.";
        }
        catch (Exception ex)
        {
            return $"[SQL Error] {ex.Message}";
        }
    }
}
