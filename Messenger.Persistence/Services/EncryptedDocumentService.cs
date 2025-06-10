using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;

namespace Messenger.Persistence.Services;

public class EncryptedDocumentService : IEncryptedDocumentService
    {
        private readonly IEncryptedDocumentRepository _documentRepository;
        private readonly ISecureChatRepository _chatRepository;
        private readonly ISecureChatParticipantRepository _participantRepository;

        public EncryptedDocumentService(
            IEncryptedDocumentRepository documentRepository,
            ISecureChatRepository chatRepository,
            ISecureChatParticipantRepository participantRepository)
        {
            _documentRepository = documentRepository;
            _chatRepository = chatRepository;
            _participantRepository = participantRepository;
        }

        public async Task UploadDocumentAsync(Guid chatId, Guid uploaderId, byte[] fileData, byte[] iv, byte[] tag, string fileName, string fileType)
        {
            if (uploaderId != Guid.Empty)
                throw new ValidationException("uploaderId", "Идентификатор загрузившего не может быть пустым");
            if (fileData == null || fileData.Length == 0)
                throw new ValidationException("fileData", "Файл не может быть пустым");
            if (fileData.Length > 10 * 1024 * 1024) // Ограничение 10 МБ
                throw new ValidationException("fileData", "Файл слишком большой");
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ValidationException("fileName", "Имя файла не может быть пустым");
            var allowedTypes = new[] { "pdf", "docx", "jpg" };
            if (!allowedTypes.Contains(fileType.ToLower()))
                throw new ValidationException("fileType", "Недопустимый тип файла");

            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
                throw new NotFoundException("SecureChat", chatId);

            if (chat.DestroyAt <= DateTime.UtcNow)
                throw new BusinessRuleException("Срок действия чата истек");

            if (!await _participantRepository.IsParticipantAsync(chatId, uploaderId))
                throw new AccessDeniedException("загрузка документа", chatId, uploaderId);

            var document = new EncryptedDocument
            {
                SecureChatId = chatId,
                UploaderId = uploaderId,
                FileData = fileData,
                IV = iv,
                Tag = tag,
                FileName = fileName,
                FileType = fileType,
                FileSize = fileData.Length,
                Timestamp = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(document);
        }

        public async Task<EncryptedDocument> GetDocumentAsync(Guid documentId, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ValidationException("userId", "Идентификатор пользователя не может быть пустым");

            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
                throw new NotFoundException("EncryptedDocument", documentId);

            var chat = await _chatRepository.GetByIdAsync(document.SecureChatId);
            if (chat == null)
                throw new NotFoundException("SecureChat", document.SecureChatId);

            if (chat.DestroyAt <= DateTime.UtcNow)
                throw new BusinessRuleException("Срок действия чата истек");

            if (!await _participantRepository.IsParticipantAsync(chat.Id, userId))
                throw new AccessDeniedException("получение документа", document.SecureChatId, userId);

            return document;
        }
        
        public async Task<IReadOnlyList<EncryptedDocumentDto>> GetDocumentsAsync(Guid chatId, Guid userId, int skip = 0, int take = 100, DateTime? timeStamp = null)
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

            return await _documentRepository.GetByChatAsync(chatId, skip, take, timeStamp);
        }
    }