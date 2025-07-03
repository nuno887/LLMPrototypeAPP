using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Core;

public enum Tool
{
    SQL,
    RAG,
    NORMAL
}

public class Agent
{
    private readonly LLM_RAG _ragLLM;
    private readonly LLM_Normal _normalLLM;
    private readonly VectorDatabaseService _vectorService;

    public Agent(
        string dbConnectionString,
        string sqlModelName,
        string normalModelName,
        string vectorDbConnectionString,
        string ragModelName)
    {
        //_sqlLLM = new LLM_SQL(dbConnectionString, sqlModelName);
        _ragLLM = new LLM_RAG(ragModelName);
        _normalLLM = new LLM_Normal(normalModelName);
        _vectorService = new VectorDatabaseService(vectorDbConnectionString);
    }

    public async Task<LLMResult> AskAsync(string question, ConversationContext context)
    {
        var tools = await DecideTools(question);
        context.AppendNotes($"[Tool Decision]: {string.Join(", ", tools)}");

        if (tools.Contains(Tool.SQL))
            await RunSQL(question, context);

        if (tools.Contains(Tool.RAG))
        {
            string confidence = await NotesContainAnswerConfidence(question, context);
            context.AppendNotes($"[Notes Sufficiency Confidence]: {confidence}");

            if (confidence == "YES")
                context.AppendNotes("[RAG Skipped - Notes sufficient]");
            else
                await RunRAG(question, context);
        }

        if (tools.Contains(Tool.NORMAL))
            await RunNormal(question, context);

        if (!context.HasAnswer)
            await RunFallback(question, context);

        // Only append essential, user-facing response
        string cleanAnswer = context.GetCleanAnswer();
        context.AppendConversation(question, cleanAnswer);

        return new LLMResult
        {
            Notes = context.GetNotes(),
            Answer = cleanAnswer
        };
    }

    private async Task<List<Tool>> DecideTools(string question)
    {
        string prompt = $@"
You are an expert AI assistant deciding which specialized tools should answer the user's question.

Available tools:
- RAG Tool: ONLY use this if the user's question requires external knowledge, document retrieval, database lookup, or facts the assistant cannot answer from general reasoning alone.
- NORMAL Tool: Use this for general reasoning, conversation, or questions that can be answered without external sources.

You should prefer the NORMAL Tool when possible. Only use the RAG Tool if absolutely necessary.

You may select one or more of the following: SQL, RAG, NORMAL. Separate choices with plus signs (+).

User question:
{question}";


        var decisionResult = await _normalLLM.ProcessQuestionAsync(prompt);
        string decisionText = decisionResult.Answer.Trim().ToUpperInvariant();

        var tools = new List<Tool>();

        if (decisionText.Contains("SQL")) tools.Add(Tool.SQL);
        if (decisionText.Contains("RAG")) tools.Add(Tool.RAG);
        if (decisionText.Contains("NORMAL")) tools.Add(Tool.NORMAL);

        return tools;
    }

    private async Task RunSQL(string question, ConversationContext context)
    {
        context.AppendNotes("[SQL Notes]");
        // Future SQL logic
    }

    private async Task RunRAG(string question, ConversationContext context)
    {
        var contextDocs = await _vectorService.SearchAsync(question, topK: 5);
        string combinedContext = contextDocs != null ? string.Join("\n", contextDocs) : string.Empty;

        var ragResult = await _ragLLM.ProcessQuestionAsync(question, combinedContext);

        context.AppendNotes("[RAG Notes]");
        context.AppendNotes(ragResult.Notes);
        context.AppendAnswer(ragResult.Answer);
    }

    private async Task RunNormal(string question, ConversationContext context)
    {
        var normalResult = await _normalLLM.ProcessQuestionAsync(question, context.GetNotes());

        context.AppendNotes("[NORMAL Notes]");
        context.AppendNotes(normalResult.Notes);
        context.AppendAnswer(normalResult.Answer);
    }

    private async Task RunFallback(string question, ConversationContext context)
    {
        context.AppendNotes("[Fallback to RAG]");

        var contextDocs = await _vectorService.SearchAsync(question, topK: 5);
        string combinedContext = contextDocs != null ? string.Join("\n", contextDocs) : string.Empty;

        var ragResult = await _ragLLM.ProcessQuestionAsync(question, combinedContext);

        context.AppendNotes(ragResult.Notes);
        context.AppendAnswer(ragResult.Answer);
    }

    private async Task<string> NotesContainAnswerConfidence(string question, ConversationContext context)
    {
        string prompt = $@"
Determine if the notes answer the question.

Reply with:
- YES: Notes fully answer.
- MAYBE: Notes partially answer.
- NO: Notes insufficient.

Notes:
{context.GetNotes()}

Question:
{question}";

        var result = await _normalLLM.ProcessQuestionAsync(prompt);
        return result.Answer.Trim().ToUpperInvariant();
    }
}
