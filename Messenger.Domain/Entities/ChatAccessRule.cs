namespace Messenger.Domain.Entities;

public class ChatAccessRule
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }           
    public Chat Chat { get; set; }
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public int AccessMask { get; set; }
}