using Microsoft.AspNetCore.Identity;

namespace Messenger.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }

    public string CorporateKey { get; set; } = string.Empty; // 12-символьный ключ

    public string RegistrationStatus { get; set; } =
        "PendingConfirmation"; // Статусы: PendingConfirmation, PendingApproval, Approved, Rejected
    public ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    //безопасный чат
    public ICollection<SecureChatParticipant> SecureChatParticipants { get; set; } = new List<SecureChatParticipant>();
    
}