using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(cp => new { cp.ChatID, cp.UserID });

        builder.HasOne(cp => cp.Chat)
            .WithMany(c => c.ChatParticipants)
            .HasForeignKey(cp => cp.ChatID)
            .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление

        builder.HasOne(cp => cp.User)
            .WithMany(u => u.ChatParticipants)
            .HasForeignKey(cp => cp.UserID)
            .OnDelete(DeleteBehavior.Cascade); // Можно оставить каскадное удаление у пользователя

        builder.Property(cp => cp.JoinedAt)
            .IsRequired(); }
}