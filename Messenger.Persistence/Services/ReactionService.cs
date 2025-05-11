using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Persistence.Services;

public class ReactionService : IReactionService
{
    private readonly IReactionRepository _reactionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IChatAccessService _chatAccessService;

    public ReactionService(
        IReactionRepository reactionRepository,
        IMessageRepository messageRepository,
        IChatAccessService chatAccessService)
    {
        _reactionRepository = reactionRepository;
        _messageRepository = messageRepository;
        _chatAccessService = chatAccessService;
    }

    public async Task AddAsync(Guid messageId, Guid userId, ReactionType reactionType, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, ct)
                      ?? throw new NotFoundException("Сообщение", messageId);

        // Проверка прав на добавление реакции
        if (!await _chatAccessService.HasAccessAsync(message.ChatId, userId, ChatAccess.ReactMessages, ct))
            throw new BusinessRuleException("У пользователя нет прав на добавление реакций в этом чате");

        // Проверка, есть ли уже реакция от пользователя
        var existingReactions = await _reactionRepository.GetByMessageAsync(messageId, ct);
        if (existingReactions.Any(r => r.UserId == userId))
            throw new BusinessRuleException("Пользователь уже добавил реакцию к этому сообщению");

        var reaction = new MessageReaction
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            UserId = userId,
            ReactionType = reactionType,
            CreatedAt = DateTime.UtcNow
        };

        await _reactionRepository.AddAsync(reaction, ct);
    }

    public async Task RemoveAsync(Guid messageId, Guid userId, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, ct)
                      ?? throw new NotFoundException("Сообщение", messageId);

        var reactions = await _reactionRepository.GetByMessageAsync(messageId, ct);
        var reaction = reactions.FirstOrDefault(r => r.UserId == userId)
                       ?? throw new NotFoundException("Реакция", $"MessageId: {messageId}, UserId: {userId}");

        await _reactionRepository.RemoveAsync(reaction.Id, ct);
    }

    public async Task<IReadOnlyList<MessageReaction>> GetByMessageAsync(Guid messageId, Guid userId, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, ct)
                      ?? throw new NotFoundException("Сообщение", messageId);

        // Проверка прав на чтение сообщений
        if (!await _chatAccessService.HasAccessAsync(message.ChatId, userId, ChatAccess.ReadMessages, ct))
            throw new BusinessRuleException("У пользователя нет прав на просмотр реакций в этом чате");

        return await _reactionRepository.GetByMessageAsync(messageId, ct);
    }
}