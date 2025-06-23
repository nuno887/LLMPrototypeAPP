namespace MyAgent.Core;

public class Agent
{
    private readonly LLM_SQL _sqlLLM;
    private readonly LLM_RAG _ragLLM;
    private readonly LLM_Normal _normalLLM;

    public Agent(string dbPath, string sqlModelName, string ragModelName, string normalModelName)
    {
        _sqlLLM = new LLM_SQL(dbPath, sqlModelName);
        _ragLLM = new LLM_RAG(ragModelName);
        _normalLLM = new LLM_Normal(normalModelName);
    }

    public async Task<string> AskAsync(string question)
    {
        const int maxAttempts = 3;

        string toolDecision = await DecideToolWithLLM(question);

        bool useSQL = toolDecision.Contains("SQL");
        bool useRAG = toolDecision.Contains("RAG");
        bool useNORMAL = toolDecision.Contains("NORMAL");

        string combinedAnswer = "";

        if (useSQL)
            combinedAnswer += await _sqlLLM.ProcessQuestionAsync(question) + "\n";

        if (useRAG)
            combinedAnswer += await _ragLLM.ProcessQuestionAsync(question) + "\n";

        if (useNORMAL)
            combinedAnswer += await _normalLLM.ProcessQuestionAsync(question) + "\n";

        return string.IsNullOrWhiteSpace(combinedAnswer)
            ? "Unable to determine the appropriate tool(s) for this question."
            : combinedAnswer.Trim();
    }

    private async Task<string> DecideToolWithLLM(string question)
    {
        string prompt = $@"
You are an expert AI assistant deciding which specialized tools should answer the user's question.

Available tools:
- SQL Tool: for structured data, databases, table-based information
- RAG Tool: for unstructured data, text documents, knowledge retrieval
- NORMAL Tool: for general reasoning, conversation, or tasks not requiring database or document access

You can select more than one tool if appropriate. Respond with one or more of: SQL, RAG, NORMAL, separated by plus signs (+).

User question:
{question}";

        string decision = await _normalLLM.ProcessQuestionAsync(prompt);

        return decision.Trim().ToUpper();
    }
}
