using System;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Core;

public class Agent
{
    private readonly LLM_SQL _sqlLLM;
    private readonly LLM_RAG _ragLLM;
    private readonly LLM_Normal _normalLLM;
    private readonly VectorDatabaseService _vectorService;

    public Agent(string dbConnectionString, string sqlModelName, string normalModelName, string vectorDbConnectionString)
    {
        _sqlLLM = new LLM_SQL(dbConnectionString, sqlModelName);
        _ragLLM = new LLM_RAG(vectorDbConnectionString);
        _normalLLM = new LLM_Normal(normalModelName);
        _vectorService = new VectorDatabaseService(vectorDbConnectionString);
    }

    public async Task<LLMResult> AskAsync(string question)
    {
        string toolDecision = await DecideToolWithLLM(question);

        bool useSQL = toolDecision.Contains("SQL");
        bool useRAG = toolDecision.Contains("RAG");
        bool useNORMAL = toolDecision.Contains("NORMAL");

        var finalNotes = new StringBuilder();
        var finalAnswer = new StringBuilder();

        if (useSQL)
        {
            var sqlResult = await _sqlLLM.ProcessQuestionAsync(question);
            finalNotes.AppendLine("[SQL Notes]");
            finalNotes.AppendLine(sqlResult.Notes);
            finalAnswer.AppendLine(sqlResult.Answer);
        }

        if (useRAG)
        {
            // NEW: Retrieve documents from vector database first
            var contextDocs = await _vectorService.SearchAsync(question, topK: 5);
            var ragResult = await _ragLLM.ProcessQuestionAsync(question);


            finalNotes.AppendLine("[RAG Notes]");
            finalNotes.AppendLine(ragResult.Notes);
            finalAnswer.AppendLine(ragResult.Answer);
        }

        if (useNORMAL)
        {
            var normalResult = await _normalLLM.ProcessQuestionAsync(question);
            finalNotes.AppendLine("[NORMAL Notes]");
            finalNotes.AppendLine(normalResult.Notes);
            finalAnswer.AppendLine(normalResult.Answer);
        }

        if (finalAnswer.Length == 0)
        {
            finalAnswer.Append("Unable to determine the appropriate tool(s) for this question.");
        }

        return new LLMResult
        {
            Notes = finalNotes.ToString().Trim(),
            Answer = finalAnswer.ToString().Trim()
        };
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

        var decisionResult = await _normalLLM.ProcessQuestionAsync(prompt);
        return decisionResult.Answer.Trim().ToUpper();
    }
}
