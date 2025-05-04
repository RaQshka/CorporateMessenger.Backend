using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class DocumentAccessRuleConfiguration : IEntityTypeConfiguration<DocumentAccessRule>
{
    public void Configure(EntityTypeBuilder<DocumentAccessRule> builder)
    {
        builder.HasKey(dar => dar.DocumentAccessRuleID);
        builder.Property(dar => dar.RuleDescription).IsRequired();
    }
}