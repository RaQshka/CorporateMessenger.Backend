using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Messenger.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Persistence.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IChatParticipantRepository _participantRepository;
    private readonly IChatAccessRepository _accessRepository;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public ChatService(
        IChatRepository chatRepository,
        IChatParticipantRepository participantRepository,
        IChatAccessRepository accessRepository,
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        _chatRepository = chatRepository;
        _participantRepository = participantRepository;
        _accessRepository = accessRepository;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Chat> CreateAsync(string name, ChatTypes type, Guid creatorId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Имя чата", "Имя чата не может быть пустым");

        var creator = await _userManager.FindByIdAsync(creatorId.ToString())
                      ?? throw new NotFoundException("Пользователь", creatorId);

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            ChatName = name,
            ChatType = (int)type,
            CreatedBy = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _chatRepository.CreateAsync(chat, ct);

        await _participantRepository.AddAsync(new ChatParticipant
        {
            ChatId = created.Id,
            UserId = creatorId,
            IsAdmin = true,
            JoinedAt = DateTime.UtcNow
        }, ct);

        var roles = _roleManager.Roles.ToList();
        foreach (var role in roles)
        {
            await _accessRepository.AddRuleAsync(new ChatAccessRule
            {
                Id = Guid.NewGuid(),
                ChatId = created.Id,
                RoleId = role.Id,
                AccessMask = (int)(ChatAccess.ReadMessages |
                                   ChatAccess.WriteMessages |
                                   ChatAccess.ReactMessages)
            }, ct);
        }

        return created;
    }

    public Task<Chat?> GetByIdAsync(Guid chatId, CancellationToken ct)
        => _chatRepository.GetByIdAsync(chatId, ct);

    public Task<IReadOnlyList<Chat>> ListByUserAsync(Guid userId, CancellationToken ct)
        => _chatRepository.ListByUserAsync(userId, ct)
            .ContinueWith(t => (IReadOnlyList<Chat>)t.Result, ct);

    public async Task RenameAsync(Guid chatId, Guid userId, string newName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Новое имя чата", "Новое имя чата не может быть пустым");

        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        chat.ChatName = newName;
        await _chatRepository.UpdateAsync(chat, ct);
    }

    public async Task DeleteAsync(Guid chatId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        await _chatRepository.DeleteAsync(chatId, ct);
    }
}