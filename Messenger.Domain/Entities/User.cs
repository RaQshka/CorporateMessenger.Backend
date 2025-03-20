namespace Messenger.Domain;

public class User
{
    public Guid UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    public string CorporateKey { get; set; } = string.Empty; // 12-символьная строка
    public string RegistrationStatus { get; set; } = "PendingConfirmation"; // Статусы: PendingConfirmation, PendingApproval, Approved, Rejected
    public bool EmailConfirmed { get; set; } = false;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}