using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.DocumentID);

        builder.HasOne(d => d.Chat)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ChatID)
            .OnDelete(DeleteBehavior.Cascade); // Изменено на Cascade

        builder.HasOne(d => d.Uploader)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UploaderID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.FileName).HasMaxLength(255).IsRequired();
        builder.Property(d => d.FileType).IsRequired();
        builder.Property(d => d.FileSize).IsRequired();
        builder.Property(d => d.FilePath).IsRequired();
        builder.Property(d => d.UploadedAt).IsRequired();
    }}