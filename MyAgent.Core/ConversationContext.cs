using System.Text;

namespace MyAgent.Core
{
    public class ConversationContext
    {
        public Guid ConversationId { get; private set; }
        public DateTime StartedAt { get; private set; }
        private readonly StringBuilder _notes = new StringBuilder();
        private readonly StringBuilder _answer = new StringBuilder();
        private readonly StringBuilder _conversation = new StringBuilder();

        public void AppendConversation(string userQuestion, string assistantAnswer)
        {
            _conversation.AppendLine($"[User]: {userQuestion}");
            _conversation.AppendLine($"[Assistant]: {assistantAnswer}");
        }

        public string GetConversation() => _conversation.ToString().Trim();

        public ConversationContext()
        {
            ConversationId = Guid.NewGuid();
            StartedAt = DateTime.Now;
        }

        public void AppendNotes(string text)
        {
            _notes.AppendLine(text);
        }

        public void AppendAnswer(string text)
        {
            _answer.AppendLine(text);
        }

        public string GetNotes() => _notes.ToString().Trim();
        public string GetAnswer() => _answer.ToString().Trim();
        public bool HasAnswer => _answer.Length > 0;
    }
}
