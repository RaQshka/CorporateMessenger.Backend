namespace Messenger.Domain;

public class Chat
{
    public Guid Id { get; set; }                       // Ранее ChatID
    public string ChatName { get; set; } = string.Empty;
    // Можно сделать ChatType перечислением, но пока оставим строкой
    public string ChatType { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ChatAccessRule> ChatAccessRules { get; set; } = new List<ChatAccessRule>();
}