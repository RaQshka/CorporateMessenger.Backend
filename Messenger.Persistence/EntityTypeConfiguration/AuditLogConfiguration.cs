﻿using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence.EntityTypeConfiguration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.LogID);
        builder.Property(a => a.ActionType).IsRequired().HasMaxLength(100);
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserID);
    }
}