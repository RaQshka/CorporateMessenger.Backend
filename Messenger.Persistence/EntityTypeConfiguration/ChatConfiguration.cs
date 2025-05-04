using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ChatName).IsRequired().HasMaxLength(200);
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.CreatedBy);
        // Конфигурация связи Chat -> ChatParticipant с каскадным удалением
        
        builder.HasMany(c => c.ChatParticipants)
            .WithOne(cp => cp.Chat)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление

        // Конфигурация связи Chat -> ChatAccessRule с каскадным удалением
        builder.HasMany(c => c.ChatAccessRules)
            .WithOne(car => car.Chat)
            .HasForeignKey(car => car.ChatId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
        // Конфигурация связи Chat -> ChatAccessRule с каскадным удалением
        builder.HasMany(c => c.Messages)
            .WithOne(car => car.Chat)
            .HasForeignKey(car => car.ChatId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
    }
}