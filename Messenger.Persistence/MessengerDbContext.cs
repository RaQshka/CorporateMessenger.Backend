using Messenger.Application.Interfaces;
using Messenger.Domain;
using Messenger.Persistence.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Persistence;

public class MessengerDbContext: DbContext, IMessengerDbContext
{
    public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
        :base(options){}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<DocumentAccessRule> DocumentAccessRules { get; set; }
    public DbSet<DocumentAccessRuleRole> DocumentAccessRuleRoles { get; set; }
    public DbSet<ChatAccessRule> ChatAccessRules { get; set; }
    public DbSet<ChatAccessRuleRole> ChatAccessRuleRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());    
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentAccessRuleRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ChatAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new ChatAccessRuleRoleConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
