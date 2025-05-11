using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        
        // Первичный ключ
        builder.HasKey(d => d.Id);

        // Свойства
        builder.Property(d => d.FileName).HasMaxLength(255).IsRequired();
        builder.Property(d => d.FileType).IsRequired();
        builder.Property(d => d.FileSize).IsRequired();
        builder.Property(d => d.FilePath).IsRequired();
        builder.Property(d => d.UploadedAt).IsRequired();
        builder.Property(d => d.ExpiresAt);
    
        // Отношение с Chat (один ко многим)
        builder.HasOne(d => d.Chat)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Отношение с Uploader (User) (один ко многим)
        builder.HasOne(d => d.Uploader)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UploaderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Отношение с Message (опционально, один ко многим)
        builder.HasOne(d => d.Message)
            .WithMany() // У Message нет коллекции Documents
            .HasForeignKey(d => d.MessageId)
            .OnDelete(DeleteBehavior.SetNull); // Если сообщение удаляется, Document остаётся, но MessageId становится null
    }}