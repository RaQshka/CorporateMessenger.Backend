using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Interfaces;

public interface IChatAccessRepository
{
    Task<ChatAccessRule> AddRuleAsync(ChatAccessRule rule, CancellationToken ct);
    Task<ChatAccessRule?> GetRuleAsync(Guid chatId, Guid roleId, CancellationToken ct);
    Task<ChatAccessRule?> GetRuleAsync(Guid chatId, string roleName, CancellationToken ct);
    Task<List<ChatAccessRule>> ListByChatAsync(Guid chatId, CancellationToken ct);
    Task UpdateRuleAsync(ChatAccessRule rule, CancellationToken ct);
    Task RemoveRuleAsync(Guid chatId, Guid roleId, CancellationToken ct);
}