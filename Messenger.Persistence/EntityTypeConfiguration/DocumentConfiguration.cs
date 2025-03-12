using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.DocumentID);
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.FilePath).IsRequired();
        builder.HasOne(d => d.Chat).WithMany(c => c.Documents).HasForeignKey(d => d.ChatID);
        builder.HasOne(d => d.Uploader).WithMany(u => u.Documents).HasForeignKey(d => d.UploaderID);
    }
}