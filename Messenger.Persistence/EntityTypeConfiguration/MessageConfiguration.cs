using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.MessageID);
        builder.Property(m => m.Content).IsRequired().HasMaxLength(5000);
        builder.HasOne(m => m.Chat).WithMany(c => c.Messages).HasForeignKey(m => m.ChatID);
        builder.HasOne(m => m.Sender).WithMany(u => u.Messages).HasForeignKey(m => m.SenderID);
    }
}