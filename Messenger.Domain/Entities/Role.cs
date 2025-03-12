namespace Messenger.Domain;

public class Role
{
    public Guid RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<ChatAccessRuleRole> ChatAccessRuleRoles { get; set; } = new List<ChatAccessRuleRole>();
    public ICollection<DocumentAccessRuleRole> DocumentAccessRuleRoles { get; set; } = new List<DocumentAccessRuleRole>();
}