using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatAccessRuleConfiguration : IEntityTypeConfiguration<ChatAccessRule>
{
    public void Configure(EntityTypeBuilder<ChatAccessRule> builder)
    {
        builder.HasKey(car => car.ChatAccessRuleID);
        builder.Property(car => car.RuleDescription).IsRequired();
    }
}