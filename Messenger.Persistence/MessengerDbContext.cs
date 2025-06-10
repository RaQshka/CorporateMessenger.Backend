﻿using Messenger.Application.Interfaces;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Persistence.EntityTypeConfiguration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence;

public class MessengerDbContext: IdentityDbContext<User, Role, Guid>, IMessengerDbContext
{
    public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageReaction> MessageReactions { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<DocumentAccessRule> DocumentAccessRules { get; set; }
    public DbSet<ChatAccessRule> ChatAccessRules { get; set; }
   
    public DbSet<SecureChatParticipant> SecureChatParticipants { get; set; }
    public DbSet<SecureChat> SecureChats { get; set; }
    public DbSet<EncryptedMessage> EncryptedMessages { get; set; }
    public DbSet<EncryptedDocument> EncryptedDocuments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());    
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new MessageReactionsConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new ChatAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new SecureChatConfiguration());
        
    }
}
