namespace Messenger.Domain;

public class ChatAccessRule
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }           
    public Chat Chat { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsAdmin { get; set; }

    public ICollection<ChatAccessRuleRole> ChatAccessRuleRoles { get; set; } = new List<ChatAccessRuleRole>();
}