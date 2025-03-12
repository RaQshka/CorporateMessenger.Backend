using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatAccessRuleRoleConfiguration : IEntityTypeConfiguration<ChatAccessRuleRole>
{
    public void Configure(EntityTypeBuilder<ChatAccessRuleRole> builder)
    {
        builder.HasKey(carr => new { carr.ChatAccessRuleID, carr.RoleID });
        builder.HasOne(carr => carr.Role).WithMany().HasForeignKey(carr => carr.RoleID);
    }
}