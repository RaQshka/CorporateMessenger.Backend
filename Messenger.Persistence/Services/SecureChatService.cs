using System.Security.Cryptography;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;

namespace Messenger.Persistence.Services;

public class SecureChatService : ISecureChatService
{
    private readonly ISecureChatRepository _chatRepository;
    private readonly ISecureChatParticipantRepository _participantRepository;

    public SecureChatService(ISecureChatRepository chatRepository,
        ISecureChatParticipantRepository participantRepository)
    {
        _chatRepository = chatRepository;
        _participantRepository = participantRepository;
    }

    public async Task<(string AccessKey, byte[] Salt, byte[] CreatorPublicKey)> CreateChatAsync(
        string name, Guid creatorId, Guid invitedUserId, DateTime destroyAt, byte[] creatorPublicKey)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("name", "Название чата не может быть пустым");
        if (creatorId == Guid.Empty)
            throw new ValidationException("creatorId", "Идентификатор создателя не может быть пустым");
        if (invitedUserId == Guid.Empty)
            throw new ValidationException("invitedUserId", "Идентификатор приглашенного пользователя не может быть пустым");
        if (destroyAt <= DateTime.UtcNow)
            throw new ValidationException("destroyAt", "Время уничтожения чата должно быть в будущем");
        if (creatorPublicKey == null || creatorPublicKey.Length == 0)
            throw new ValidationException("creatorPublicKey", "Публичный ключ создателя не может быть пустым");

        var accessKey = Guid.NewGuid().ToString();
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        var chat = new SecureChat()
        {
            Name = name,
            CreatedAt = DateTime.UtcNow,
            DestroyAt = destroyAt,
            AccessKey = accessKey,
            Salt = salt,
        };

        await _chatRepository.CreateAsync(chat);

        // Добавляем создателя с публичным ключом
        var creatorParticipant = new SecureChatParticipant
        {
            SecureChatId = chat.Id,
            UserId = creatorId,
            PublicKey = creatorPublicKey
        };
        await _participantRepository.AddAsync(creatorParticipant);

        // Добавляем приглашенного пользователя с пустым публичным ключом
        var invitedParticipant = new SecureChatParticipant
        {
            SecureChatId = chat.Id,
            UserId = invitedUserId,
            PublicKey = null
        };
        await _participantRepository.AddAsync(invitedParticipant);

        return (accessKey, salt, creatorPublicKey);
    }

    public async Task<(Guid ChatId, byte[] Salt, byte[] OtherPublicKey)> EnterChatAsync(
        string accessKey, Guid userId, byte[] publicKey)
    {
        if (string.IsNullOrWhiteSpace(accessKey))
            throw new ValidationException("accessKey", "Ключ доступа не может быть пустым");
        if (userId == Guid.Empty)
            throw new ValidationException("userId", "Идентификатор пользователя не может быть пустым");
        if (publicKey == null || publicKey.Length == 0)
            throw new ValidationException("publicKey", "Публичный ключ не может быть пустым");

        var chat = await _chatRepository.GetByAccessKeyAsync(accessKey);
        if (chat == null)
            throw new NotFoundException("SecureChat", accessKey);

        if (chat.DestroyAt <= DateTime.UtcNow)
            throw new BusinessRuleException("Срок действия чата истек");

        var participant = await _participantRepository.GetByChatAndUserAsync(chat.Id, userId);
        if (participant == null)
        {
            throw new AccessDeniedException("присоединение к чату", chat.Id, userId);
        }

        // Обновление ключа существующего участника
        participant.PublicKey = publicKey;
        await _participantRepository.UpdateAsync(participant);

        var participants = await _participantRepository.GetParticipantsAsync(chat.Id);
        var otherParticipant = participants.FirstOrDefault(x=>x.UserId != userId);
        
        if (otherParticipant == null)
            throw new NotFoundException("SecureChatParticipant", "Второй собеседник не найден");

        return (chat.Id, chat.Salt, otherParticipant.PublicKey);
    }

    public async Task<bool> IsParticipantAsync(Guid chatId, Guid userId)
    {
        return await _participantRepository.IsParticipantAsync(chatId, userId);
    }
}