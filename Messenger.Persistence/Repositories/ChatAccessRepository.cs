using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class ChatAccessRepository:IChatAccessRepository
{
    private readonly IMessengerDbContext _context;
    
    public ChatAccessRepository(IMessengerDbContext context)
    {
        _context = context;
    }
    
    public async Task<ChatAccessRule> AddRuleAsync(ChatAccessRule rule, CancellationToken ct)
    {
        await _context.ChatAccessRules.AddAsync(rule, ct);
        await _context.SaveChangesAsync(ct);
        return rule;
    }

    public async Task<ChatAccessRule?> GetRuleAsync(Guid chatId, Guid roleId, CancellationToken ct)
    {
        return await _context.ChatAccessRules
            .FirstOrDefaultAsync(r => r.ChatId == chatId && r.RoleId == roleId, ct);
    }

    public async Task<ChatAccessRule?> GetRuleAsync(Guid chatId, string roleName, CancellationToken ct)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if(role == null)
            throw new ArgumentException($"Role {roleName} not found");
        
        return await _context.ChatAccessRules
            .FirstOrDefaultAsync(r => r.ChatId == chatId && r.RoleId == role.Id, ct);
    }

    public async Task<List<ChatAccessRule>> ListByChatAsync(Guid chatId, CancellationToken ct)
    {
        return await _context.ChatAccessRules
            .Include(r => r.Role)
            .Where(r => r.ChatId == chatId)
            .ToListAsync(ct);
    }

    public async Task UpdateRuleAsync(ChatAccessRule rule, CancellationToken ct)
    {
        _context.ChatAccessRules.Update(rule);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveRuleAsync(Guid chatId, Guid roleId, CancellationToken ct)
    {
        var rule = await GetRuleAsync(chatId, roleId, ct);
        if (rule != null)
        {
            _context.ChatAccessRules.Remove(rule);
            await _context.SaveChangesAsync(ct);
        }
    }

}