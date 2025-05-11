namespace Messenger.Domain.Entities;

public class DocumentAccessRule
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public int DocumentAccessMask { get; set; }
}