using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class DocumentAccessRuleRoleConfiguration : IEntityTypeConfiguration<DocumentAccessRuleRole>
{
    public void Configure(EntityTypeBuilder<DocumentAccessRuleRole> builder)
    {
        builder.HasKey(darr => new { darr.DocumentAccessRuleID, darr.RoleID });

        builder.HasOne(darr => darr.DocumentAccessRule)
            .WithMany()
            .HasForeignKey(darr => darr.DocumentAccessRuleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(darr => darr.Role)
            .WithMany(r => r.DocumentAccessRuleRoles)
            .HasForeignKey(darr => darr.RoleID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}