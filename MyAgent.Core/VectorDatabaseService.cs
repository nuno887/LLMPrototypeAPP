using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyAgent.Core
{
    public class VectorDatabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _vectorApiUrl;

        public VectorDatabaseService(string vectorApiUrl)
        {
            _httpClient = new HttpClient();
            _vectorApiUrl = vectorApiUrl;
        }

        public async Task<List<string>> SearchAsync(string query, int topK = 3)
        {
            var payload = JsonSerializer.Serialize(new { query, top_k = topK });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_vectorApiUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var results = new List<string>();

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("documents", out var docs))
            {
                foreach (var item in docs.EnumerateArray())
                {
                    results.Add(item.GetString() ?? string.Empty);
                }
            }

            return results;
        }
    }

}
