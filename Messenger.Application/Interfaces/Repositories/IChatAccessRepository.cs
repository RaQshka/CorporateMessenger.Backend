using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IChatAccessRepository
{
    Task<ChatAccessRule> AddRuleAsync(ChatAccessRule rule, CancellationToken ct);
    Task<ChatAccessRule?> GetRuleAsync(Guid chatId, Guid roleId, CancellationToken ct);
    Task<ChatAccessRule?> GetRuleAsync(Guid chatId, string roleName, CancellationToken ct);
    Task<List<ChatAccessRule>> ListByChatAsync(Guid chatId, CancellationToken ct);
    Task UpdateRuleAsync(ChatAccessRule rule, CancellationToken ct);
    Task RemoveRuleAsync(Guid chatId, Guid roleId, CancellationToken ct);
}