using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class MessageReactionsConfiguration : IEntityTypeConfiguration<MessageReaction>
{
    public void Configure(EntityTypeBuilder<MessageReaction> builder)
    {
// Первичный ключ
        builder.HasKey(r => r.Id);

        // Свойства
        builder.Property(r => r.ReactionType).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        // Отношение с Message (один ко многим)
        builder.HasOne(r => r.Message)
            .WithMany(m => m.Reactions)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Отношение с User (один ко многим)
        builder.HasOne(r => r.User)
            .WithMany() // У User нет коллекции MessageReactions
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}