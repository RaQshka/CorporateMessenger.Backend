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

        // Связь Chat -> ChatParticipants с каскадным удалением
        builder.HasMany(c => c.ChatParticipants)
            .WithOne(cp => cp.Chat)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Chat -> ChatAccessRules с каскадным удалением
        builder.HasMany(c => c.ChatAccessRules)
            .WithOne(car => car.Chat)
            .HasForeignKey(car => car.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Chat -> Messages с каскадным удалением
        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Chat -> Documents с каскадным удалением
        builder.HasMany(c => c.Documents)
            .WithOne(d => d.Chat)
            .HasForeignKey(d => d.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}