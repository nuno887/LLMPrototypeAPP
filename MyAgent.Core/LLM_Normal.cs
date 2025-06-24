namespace MyAgent.Core;

public class LLM_Normal
{
    private readonly OllamaClient _ollama;

    public LLM_Normal(string modelName)
    {
        _ollama = new OllamaClient(modelName);
    }

    public async Task<LLMResult> ProcessQuestionAsync(string question)
    {
        string prompt = $@"
You are a helpful AI assistant. Answer the user's question clearly and concisely.

User question:
{question}";

        string result = await _ollama.AskAsync(prompt);

        string cleanAnswer = result;

        // Attempt to extract clean text if JSON is returned
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(result);
            if (doc.RootElement.TryGetProperty("response", out var responseElement))
            {
                cleanAnswer = responseElement.GetString() ?? "[Empty response]";
            }
        }
        catch
        {
            // If not valid JSON, assume plain text answer
        }

        return new LLMResult
        {
            Notes = result,      // Keep raw LLM output as Notes
            Answer = cleanAnswer // Cleaned, readable answer
        };
    }
}
