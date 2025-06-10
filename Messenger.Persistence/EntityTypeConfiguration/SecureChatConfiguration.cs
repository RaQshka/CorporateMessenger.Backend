using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class SecureChatConfiguration : IEntityTypeConfiguration<SecureChat>
{
    public void Configure(EntityTypeBuilder<SecureChat> builder)
    {
        builder
            .HasMany(sc => sc.Participants)
            .WithOne(p => p.SecureChat)
            .HasForeignKey(p => p.SecureChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(sc => sc.Messages)
            .WithOne(m => m.SecureChat)
            .HasForeignKey(m => m.SecureChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(sc => sc.Documents)
            .WithOne(d => d.SecureChat)
            .HasForeignKey(d => d.SecureChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

