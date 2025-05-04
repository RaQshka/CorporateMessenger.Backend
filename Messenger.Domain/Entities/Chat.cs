namespace Messenger.Domain.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public string ChatType { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ChatAccessRule> ChatAccessRules { get; set; } = new List<ChatAccessRule>();
}