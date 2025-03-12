namespace Messenger.Domain;

public class UserRole
{
    public Guid UserID { get; set; }
    public User User { get; set; }
    
    public Guid RoleID { get; set; }
    public Role Role { get; set; }
}