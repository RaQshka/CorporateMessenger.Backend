using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatAccessRuleConfiguration : IEntityTypeConfiguration<ChatAccessRule>
{
    public void Configure(EntityTypeBuilder<ChatAccessRule> builder)
    {
        builder.HasKey(car => car.Id);
        builder.Property(car => car.ChatId).IsRequired();
        builder.HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}