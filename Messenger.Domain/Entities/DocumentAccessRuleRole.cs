namespace Messenger.Domain;

public class DocumentAccessRuleRole
{
    public Guid DocumentAccessRuleID { get; set; }
    public DocumentAccessRule DocumentAccessRule { get; set; }
    
    public Guid RoleID { get; set; }
    public Role Role { get; set; }
}