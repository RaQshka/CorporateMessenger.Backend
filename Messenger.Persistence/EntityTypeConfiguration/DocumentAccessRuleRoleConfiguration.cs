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
        //builder.HasOne(darr => darr.Role).WithMany().HasForeignKey(darr => darr.RoleID);
    }
}