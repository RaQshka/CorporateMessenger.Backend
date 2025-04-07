using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(cp => new { cp.ChatId, cp.UserId });

        builder.HasOne(cp => cp.Chat)
            .WithMany(c => c.ChatParticipants)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление

        builder.HasOne(cp => cp.User)
            .WithMany(u => u.ChatParticipants)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Можно оставить каскадное удаление у пользователя

        builder.Property(cp => cp.JoinedAt)
            .IsRequired(); }
}