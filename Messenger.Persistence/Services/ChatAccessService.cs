using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Persistence.Services;

public class ChatAccessService : IChatAccessService
{
    private readonly IChatAccessRepository _chatAccessRepository;
    private readonly IChatRepository _chatRepository;
    private readonly UserManager<User> _userManager;
    private readonly IChatParticipantRepository _chatParticipantRepository;

    public ChatAccessService(
        IChatAccessRepository chatAccessRepository,
        IChatRepository chatRepository,
        IChatParticipantRepository chatParticipantRepository,
        UserManager<User> userManager)
    {
        _chatAccessRepository = chatAccessRepository;
        _chatRepository = chatRepository;
        _chatParticipantRepository = chatParticipantRepository;
        _userManager = userManager;
    }

    public async Task<bool> HasAccessAsync(Guid chatId, Guid userId, ChatAccess accessFlag, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException("Пользователь", userId);

        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        if (chat.CreatedBy == userId || await IsAdminOfChat(chatId, userId, ct))
            return true;

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0) return false;

        int combinedMask = 0;
        foreach (var role in roles)
        {
            var chatAccess = await _chatAccessRepository.GetRuleAsync(chatId, role, ct);
            if (chatAccess != null)
                combinedMask = chatAccess.AccessMask | combinedMask;
        }
        return (combinedMask & (int)accessFlag) == (int)accessFlag;
    }

    public async Task SetMaskAsync(Guid chatId, Guid roleId, int mask, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var existing = await _chatAccessRepository.GetRuleAsync(chatId, roleId, ct);
        if (existing == null)
        {
            await _chatAccessRepository.AddRuleAsync(new ChatAccessRule
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                RoleId = roleId,
                AccessMask = mask,
            }, ct);
        }
        else
        {
            existing.AccessMask = mask;
            await _chatAccessRepository.UpdateRuleAsync(existing, ct);
        }
    }
    public async Task RemoveAsync(Guid chatId, Guid roleId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var existing = await _chatAccessRepository.GetRuleAsync(chatId, roleId, ct)
                       ?? throw new NotFoundException("Правило доступа", $"ChatId: {chatId}, RoleId: {roleId} ");

        await _chatAccessRepository.RemoveRuleAsync(chatId, roleId, ct);
    }

    public async Task<IReadOnlyList<ChatAccessRule>> GetAllAsync(Guid chatId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        return await _chatAccessRepository.ListByChatAsync(chatId, ct);
    }
    public async Task GrantAccessAsync(Guid chatId, Guid roleId, ChatAccess rights, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var existing = await _chatAccessRepository.GetRuleAsync(chatId, roleId, ct)
                       ?? throw new NotFoundException("Правило доступа", $"ChatId: {chatId}, RoleId: {roleId}");

        var newMask = existing.AccessMask | (int)rights;
        await _chatAccessRepository.UpdateRuleAsync(
            new ChatAccessRule { ChatId = chatId, RoleId = roleId, AccessMask = newMask },
            ct);
    }
    public async Task RevokeAccessAsync(Guid chatId, Guid roleId, ChatAccess rights, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var existing = await _chatAccessRepository.GetRuleAsync(chatId, roleId, ct)
                       ?? throw new NotFoundException("Правило доступа", $"ChatId: {chatId}, RoleId: {roleId}");

        var updatedMask = existing.AccessMask & ~(int)rights;
        var chatAccessRule = new ChatAccessRule
        {
            ChatId = chatId,
            RoleId = roleId,
            AccessMask = updatedMask
        };
        await _chatAccessRepository.UpdateRuleAsync(chatAccessRule, ct);
    }

    public async Task<bool> IsAdminOfChat(Guid chatId, Guid userId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var participants = await _chatParticipantRepository.ListByChatAsync(chatId, ct);
        return participants.Any(p => p.UserId == userId && p.IsAdmin);
    }
}