using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyAgent.Core;

public class LLM_RAG
{
    private readonly HttpClient _httpClient;
    private readonly string _ragApiUrl;

    public LLM_RAG(string ragApiUrl)
    {
        _httpClient = new HttpClient();
        _ragApiUrl = ragApiUrl;
    }

    public async Task<string> ProcessQuestionAsync(string question)
    {
        string prompt = $@"
You are an AI assistant that retrieves relevant documents to help answer the user's question.
You have access to a knowledge base of text files and metadata.
Focus on finding the most relevant information based on the user's intent.

User question:
{question}";

        var payload = JsonSerializer.Serialize(new { question = prompt });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_ragApiUrl, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(result);
                if (doc.RootElement.TryGetProperty("response", out var responseElement))
                {
                    return responseElement.GetString() ?? "[RAG returned empty]";
                }
                return "[Unexpected RAG response structure]";
            }
            catch
            {
                return $"[Invalid JSON from RAG] {result}";
            }
        }
        catch (Exception ex)
        {
            return $"[RAG Error] {ex.Message}";
        }
    }
}
