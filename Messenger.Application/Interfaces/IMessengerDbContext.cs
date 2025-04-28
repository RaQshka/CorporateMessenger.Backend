using Messenger.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Application.Interfaces;

public interface IMessengerDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Chat> Chats { get; set; }
    DbSet<ChatParticipant> ChatParticipants { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<Document> Documents { get; set; }
    DbSet<AuditLog> AuditLogs { get; set; }
    DbSet<Role> Roles { get; set; }
    //DbSet<UserRole> UserRoles { get; set; }
    DbSet<DocumentAccessRule> DocumentAccessRules { get; set; }
    DbSet<DocumentAccessRuleRole> DocumentAccessRuleRoles { get; set; }
    DbSet<ChatAccessRule> ChatAccessRules { get; set; }
    /*
    DbSet<ChatAccessRuleRole> ChatAccessRuleRoles { get; set; }
    */
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}