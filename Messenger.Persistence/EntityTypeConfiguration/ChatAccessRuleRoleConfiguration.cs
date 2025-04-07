using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatAccessRuleRoleConfiguration : IEntityTypeConfiguration<ChatAccessRuleRole>
{
    public void Configure(EntityTypeBuilder<ChatAccessRuleRole> builder)
    {
        builder.HasKey(carr => new { carr.ChatAccessRuleID, carr.RoleID });

        builder.HasOne(carr => carr.ChatAccessRule)
            .WithMany(car => car.ChatAccessRuleRoles)
            .HasForeignKey(carr => carr.ChatAccessRuleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(carr => carr.Role)
            .WithMany(r => r.ChatAccessRuleRoles)
            .HasForeignKey(carr => carr.RoleID)
            .OnDelete(DeleteBehavior.Cascade); // Можно заменить на Restrict, если проблемы останутся
    }
}