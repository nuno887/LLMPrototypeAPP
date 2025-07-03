using System.Text;

public class ConversationContext
{
    private readonly StringBuilder _notes = new StringBuilder();
    private readonly StringBuilder _answer = new StringBuilder();
    private readonly StringBuilder _conversation = new StringBuilder();

    // Existing methods ...

    public void AppendNotes(string text) => _notes.AppendLine(text);
    public void AppendAnswer(string text) => _answer.AppendLine(text);

    public string GetNotes() => _notes.ToString().Trim();
    public string GetAnswer() => _answer.ToString().Trim();
    public string GetConversation() => _conversation.ToString().Trim();
    public bool HasAnswer => _answer.Length > 0;

    public void AppendConversation(string userQuestion, string assistantAnswer)
    {
        _conversation.AppendLine("─────────────────────────────────────────────");
        _conversation.AppendLine($"👤 User: {userQuestion}");
        _conversation.AppendLine();
        _conversation.AppendLine($"🤖 Assistant:\n{assistantAnswer}");
        _conversation.AppendLine("─────────────────────────────────────────────\n");
    }


    /// <summary>
    /// Returns only the essential assistant reply for the user.
    /// Strips technical notes or prefixes if present.
    /// </summary>
    public string GetCleanAnswer()
    {
        var lines = _answer.ToString()
                           .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                           .Select(line => line.Trim())
                           .Where(line => !line.StartsWith("[") || !line.EndsWith("Notes]"))
                           .ToList();

        return string.Join("\n", lines);
    }

}
