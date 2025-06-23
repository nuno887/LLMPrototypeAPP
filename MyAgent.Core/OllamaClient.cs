using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyAgent.Core;

public class OllamaClient
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _modelName;

    public OllamaClient(string modelName)
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        _modelName = modelName;
    }

    public async Task<string> AskAsync(string prompt)
    {
        var json = JsonSerializer.Serialize(new
        {
            model = _modelName,
            prompt = prompt,
            stream = false
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}
