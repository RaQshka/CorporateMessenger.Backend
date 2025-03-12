namespace Messenger.Domain;

public class DocumentAccessRule
{
    public Guid DocumentAccessRuleID { get; set; }
    public Guid DocumentID { get; set; }
    public Document Document { get; set; }
    public string RuleDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}