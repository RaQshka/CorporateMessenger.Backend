using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;

namespace Messenger.Persistence.Services;

public class EncryptedMessageService : IEncryptedMessageService
    {
        private readonly IEncryptedMessageRepository _messageRepository;
        private readonly ISecureChatRepository _chatRepository;
        private readonly ISecureChatParticipantRepository _participantRepository;

        public EncryptedMessageService(
            IEncryptedMessageRepository messageRepository,
            ISecureChatRepository chatRepository,
            ISecureChatParticipantRepository participantRepository)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _participantRepository = participantRepository;
        }

        public async Task SendMessageAsync(Guid chatId, Guid senderId, byte[] ciphertext, byte[] iv, byte[] tag)
        {
            if (senderId == Guid.Empty)
                throw new ValidationException("senderId", "Идентификатор отправителя не может быть пустым");
            if (ciphertext == null || ciphertext.Length == 0)
                throw new ValidationException("ciphertext", "Сообщение не может быть пустым");
            if (ciphertext.Length > 1024 * 1024) // Ограничение 1 МБ
                throw new ValidationException("ciphertext", "Сообщение слишком большое");

            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
                throw new NotFoundException("SecureChat", chatId);

            if (chat.DestroyAt <= DateTime.UtcNow)
                throw new BusinessRuleException("Срок действия чата истек");

            if (!await _participantRepository.IsParticipantAsync(chatId, senderId))
                throw new AccessDeniedException("отправка сообщения", chatId, senderId);

            var message = new EncryptedMessage
            {
                SecureChatId = chatId,
                SenderId = senderId,
                Content = ciphertext,
                IV = iv,
                Tag = tag,
                Timestamp = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
        }

        public async Task<List<EncryptedMessageDto>> GetMessagesAsync(Guid chatId, Guid userId, int skip = 0, int take = 100)
        {
            if (userId == Guid.Empty)
                throw new ValidationException("userId", "Идентификатор пользователя не может быть пустым");
            if (skip < 0)
                throw new ValidationException("skip", "Пропуск должен быть неотрицательным");
            if (take <= 0 || take > 100)
                throw new ValidationException("take", "Количество сообщений должно быть от 1 до 100");

            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
                throw new NotFoundException("SecureChat", chatId);

            if (chat.DestroyAt <= DateTime.UtcNow)
                throw new BusinessRuleException("Срок действия чата истек");

            if (!await _participantRepository.IsParticipantAsync(chatId, userId))
                throw new AccessDeniedException("получение сообщений", chatId, userId);

            return await _messageRepository.GetByChatIdAsync(chatId, skip, take);
        }
    }