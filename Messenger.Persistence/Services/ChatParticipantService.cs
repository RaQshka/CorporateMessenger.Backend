using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Persistence.Services;

public class ChatParticipantService : IChatParticipantService
{
    private readonly IChatParticipantRepository _chatParticipantRepository;
    private readonly IChatRepository _chatRepository;
    private readonly UserManager<User> _userManager;

    public ChatParticipantService(
        IChatParticipantRepository chatParticipantRepository,
        IChatRepository chatRepository,
        UserManager<User> userManager)
    {
        _chatParticipantRepository = chatParticipantRepository;
        _chatRepository = chatRepository;
        _userManager = userManager;
    }

    public async Task AddAsync(Guid chatId, Guid userId, bool isAdmin, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException("Пользователь", userId);

        var exists = await _chatParticipantRepository.ExistsAsync(chatId, userId, ct);
        if (exists)
            throw new BusinessRuleException("Пользователь уже является участником чата");

        var chatParticipants = await _chatParticipantRepository.ListByChatAsync(chatId, ct);
        
        if (chat.ChatType == (int)ChatTypes.Dialog && chatParticipants.Count >=2)
        {
            throw new BusinessRuleException("В диалог нельзя добавить суммарно больше двух человек");
        }
        
        var entity = new ChatParticipant
        {
            ChatId = chatId,
            UserId = userId,
            IsAdmin = isAdmin,
            JoinedAt = DateTime.UtcNow
        };

        await _chatParticipantRepository.AddAsync(entity, ct);
    }


    public async Task AddByEmailAsync(Guid chatId, string userEmail, bool isAdmin, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var user = await _userManager.FindByEmailAsync(userEmail)
                   ?? throw new NotFoundException("Пользователь", userEmail);

        var exists = await _chatParticipantRepository.ExistsAsync(chatId, user.Id, ct);
        if (exists)
            throw new BusinessRuleException("Пользователь уже является участником чата");

        var chatParticipants = await _chatParticipantRepository.ListByChatAsync(chatId, ct);
        
        if (chat.ChatType == (int)ChatTypes.Dialog && chatParticipants.Count >=2)
        {
            throw new BusinessRuleException("В диалог нельзя добавить суммарно больше двух человек");
        }
        
        var entity = new ChatParticipant
        {
            ChatId = chatId,
            UserId = user.Id,
            IsAdmin = isAdmin,
            JoinedAt = DateTime.UtcNow
        };

        await _chatParticipantRepository.AddAsync(entity, ct);
    }
    public async Task RemoveAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException("Пользователь", userId);

        if (chat.CreatedBy == userId)
            throw new BusinessRuleException("Нельзя удалить создателя чата");

        await _chatParticipantRepository.RemoveAsync(chatId, userId, ct);
    }
    
    public async Task SetAdminAsync(Guid chatId, Guid userId, bool isAdmin, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException("Пользователь", userId);

        if (chat.CreatedBy == userId && !isAdmin)
            throw new BusinessRuleException("Создатель чата всегда должен быть администратором");

        await _chatParticipantRepository.SetAdminAsync(chatId, userId, isAdmin, ct);
    }


    public async Task<IReadOnlyList<ChatParticipant>> GetAllAsync(Guid chatId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var participants = await _chatParticipantRepository.ListByChatAsync(chatId, ct);
        return participants.ToList();
    }

}