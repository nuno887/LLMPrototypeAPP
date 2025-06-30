using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace MyAgent.Core
{
    public class EmbeddingGenerator
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _ollamaEndpoint = "http://localhost:11434/api/embeddings";

        public async Task<string> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var requestBody = new
            {
                model = "nomic-embed-text:latest",
                prompt = text
            };

            string jsonRequest = JsonSerializer.Serialize(requestBody);

            var response = await _httpClient.PostAsync(_ollamaEndpoint, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Ollama request failed: {response.StatusCode}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(jsonResponse);

            var embeddingArray = parsed.RootElement.GetProperty("embedding");

            // Convert to proper JSON array string
            var numbers = embeddingArray.EnumerateArray().Select(x => x.GetDouble());
            string jsonArray = JsonSerializer.Serialize(numbers);

            return jsonArray;  // Returns: [0.123456, 0.234567, 0.345678, ...]
        }

    }
}
