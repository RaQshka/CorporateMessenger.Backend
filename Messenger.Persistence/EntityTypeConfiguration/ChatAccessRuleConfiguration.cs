using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class ChatAccessRuleConfiguration : IEntityTypeConfiguration<ChatAccessRule>
{
    public void Configure(EntityTypeBuilder<ChatAccessRule> builder)
    {
        builder.HasKey(car => car.Id);

        builder.HasOne(car => car.Chat)
            .WithMany(c => c.ChatAccessRules)
            .HasForeignKey(car => car.ChatId)
            .OnDelete(DeleteBehavior.Cascade); // Изменено на Cascade для согласованности

        builder.HasOne(car => car.Role)
            .WithMany()
            .HasForeignKey(car => car.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(car => car.AccessMask).IsRequired();
    }
}