using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Persistence.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IChatAccessService _chatAccessService;
    private readonly IChatParticipantService _chatParticipantService;

    public MessageService(
        IMessageRepository messageRepository,
        IChatRepository chatRepository,
        IChatAccessService chatAccessService,
        IChatParticipantService chatParticipantService)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _chatAccessService = chatAccessService;
        _chatParticipantService = chatParticipantService;
    }

    public async Task<Guid> SendAsync(Guid chatId, Guid senderId, string content, Guid? replyToMessageId, CancellationToken ct)
    {
        // Проверка существования чата
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        // Проверка участия пользователя в чате
        var isParticipant = await _chatParticipantService.GetAllAsync(chatId, ct)
            .ContinueWith(t => t.Result.Any(p => p.UserId == senderId), ct);
        if (!isParticipant)
            throw new BusinessRuleException("Пользователь не является участником чата");

        // Проверка прав на отправку сообщений
        if (!await _chatAccessService.HasAccessAsync(chatId, senderId, ChatAccess.WriteMessages, ct))
            throw new BusinessRuleException("У пользователя нет прав на отправку сообщений в этот чат");

        // Проверка сообщения, на которое отвечаем (если указано)
        if (replyToMessageId.HasValue)
        {
            var replyMessage = await _messageRepository.GetByIdAsync(replyToMessageId.Value, ct)
                               ?? throw new NotFoundException("Сообщение для ответа", replyToMessageId.Value);
            if (replyMessage.ChatId != chatId)
                throw new BusinessRuleException("Сообщение для ответа принадлежит другому чату");
        }

        // Создание сообщения
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = content ?? string.Empty,
            ReplyToMessageId = replyToMessageId,
            SentAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _messageRepository.AddAsync(message, ct);
        return message.Id;
    }

    public async Task<IReadOnlyList<MessageDto>> GetByChatAsync(Guid chatId, Guid userId, int skip, int take, CancellationToken ct)
    {
        // Проверка существования чата
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        // Проверка прав на чтение сообщений
        if (!await _chatAccessService.HasAccessAsync(chatId, userId, ChatAccess.ReadMessages, ct))
            throw new BusinessRuleException("У пользователя нет прав на чтение сообщений в этом чате");

        // Получение сообщений
        return await _messageRepository.GetByChatAsync(chatId, skip, take, ct);
    }

    public async Task UpdateAsync(Guid messageId, Guid userId, string newContent, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, ct)
                      ?? throw new NotFoundException("Сообщение", messageId);

        // Проверка прав на редактирование (отправитель или администратор)
        var isAdmin = await _chatAccessService.IsAdminOfChat(message.ChatId, userId, ct);
        if (message.SenderId != userId && !isAdmin)
            throw new BusinessRuleException("Только отправитель или администратор могут редактировать сообщение");

        // Обновление содержимого
        message.Content = newContent ?? string.Empty;
        message.SentAt = DateTime.UtcNow;
        await _messageRepository.UpdateAsync(message, ct);
    }

    public async Task DeleteAsync(Guid messageId, Guid userId, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, ct)
                      ?? throw new NotFoundException("Сообщение", messageId);

        // Проверка прав на удаление (отправитель или администратор или пользователь с соотв. правами)
        var isAdmin = await _chatAccessService.IsAdminOfChat(message.ChatId, userId, ct);

        if (message.SenderId != userId && !isAdmin)
        {
            // Проверка прав на чтение сообщений
            if (!await _chatAccessService.HasAccessAsync(message.ChatId, userId, ChatAccess.DeleteMessage, ct))
                throw new BusinessRuleException("У пользователя нет прав на удаление чужих сообщений в этом чате");

            throw new BusinessRuleException("Только отправитель или администратор могут удалить сообщение");
        }

        // Пометка сообщения как удалённое
        message.IsDeleted = true;
        await _messageRepository.UpdateAsync(message, ct);
    }
}