using System.Text.Json;
using System.Threading.Tasks;

namespace MyAgent.Core;

public class LLM_RAG
{
    private readonly OllamaClient _ollama;

    public LLM_RAG(string modelName)
    {
        _ollama = new OllamaClient(modelName);
    }

    public async Task<LLMResult> ProcessQuestionAsync(string question, string context)
    {
        string prompt = $@"
You are a helpful assistant. Use the provided documents to answer the user's question.

Documents:
{context}

User question:
{question}

Rules:
- Only answer based on the provided documents.
- If the documents do not contain enough information, say 'Not enough information found in the provided documents'.
- Do NOT invent facts or provide unrelated answers.
";

        string response = await _ollama.AskAsync(prompt);
        string cleanResponse = ExtractResponse(response);

        return new LLMResult
        {
            Notes = $"Context used:\n{context}",
            Answer = cleanResponse
        };
    }

    private string ExtractResponse(string rawOutput)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(rawOutput);
            if (jsonDoc.RootElement.TryGetProperty("response", out var responseElement))
                return responseElement.GetString() ?? rawOutput;
        }
        catch
        {
            // Not JSON, return raw output
        }
        return rawOutput;
    }
}
