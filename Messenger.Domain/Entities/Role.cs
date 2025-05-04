using Microsoft.AspNetCore.Identity;

namespace Messenger.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    /*
    public ICollection<ChatAccessRuleRole> ChatAccessRuleRoles { get; set; } = new List<ChatAccessRuleRole>();
    */
    public ICollection<DocumentAccessRuleRole> DocumentAccessRuleRoles { get; set; } = new List<DocumentAccessRuleRole>();
}