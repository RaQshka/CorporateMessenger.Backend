namespace Messenger.Domain;

public class ChatAccessRule
{
    public Guid ChatAccessRuleID { get; set; }
    public Guid ChatID { get; set; }
    public Chat Chat { get; set; }
    public string RuleDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}