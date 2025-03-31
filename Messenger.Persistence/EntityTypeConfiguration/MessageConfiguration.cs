﻿using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.MessageID);

        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatID)
            .OnDelete(DeleteBehavior.Cascade); // Запрещаем каскадное удаление

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderID)
            .OnDelete(DeleteBehavior.Restrict); // Оставляем каскадное удаление для отправителя

        builder.Property(m => m.Content).IsRequired().HasMaxLength(5000);
        builder.Property(m => m.SentAt).IsRequired();
        builder.Property(m => m.IsDeleted).HasDefaultValue(false);
    }
}