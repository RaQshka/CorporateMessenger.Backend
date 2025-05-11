using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Messenger.Persistence.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IChatAccessService _chatAccessService;
    private readonly IChatParticipantService _chatParticipantService;
    private readonly IDocumentAccessService _documentAccessService;
    private readonly string _uploadPath;

    public DocumentService(
        IDocumentRepository documentRepository,
        IChatRepository chatRepository,
        IChatAccessService chatAccessService,
        IChatParticipantService chatParticipantService,
        IDocumentAccessService documentAccessService)
    {
        _documentRepository = documentRepository;
        _chatRepository = chatRepository;
        _chatAccessService = chatAccessService;
        _chatParticipantService = chatParticipantService;
        _documentAccessService = documentAccessService;
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<Guid> UploadAsync(Guid chatId, Guid uploaderId, IFormFile file, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        var participants = await _chatParticipantService.GetAllAsync(chatId, ct);
        if (!participants.Any(p => p.UserId == uploaderId))
            throw new BusinessRuleException("Пользователь не является участником чата");

        if (!await _chatAccessService.HasAccessAsync(chatId, uploaderId, ChatAccess.WriteMessages, ct))
            throw new BusinessRuleException("У пользователя нет прав на загрузку документов в этот чат");

        if (file == null || file.Length == 0)
            throw new ValidationException("Файл", "Файл не предоставлен");
        if (file.Length > 15 * 1024 * 1024)
            throw new ValidationException("Файл", "Размер файла превышает допустимый лимит (15 МБ)");

        var allowedExtensions = new[] { ".pdf", ".pptx", ".doc", ".docx", ".rtf", ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new ValidationException("Файл",
                $"Недопустимый тип файла. Допустимые форматы: {string.Join(", ", allowedExtensions)}");

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream, ct);
            }
        }
        catch (Exception ex)
        {
            throw new BusinessRuleException($"Ошибка при сохранении файла: {ex.Message}");
        }

        var document = new Document
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            UploaderId = uploaderId,
            FileName = file.FileName,
            FileType = file.ContentType,
            FileSize = file.Length,
            FilePath = filePath,
            UploadedAt = DateTime.UtcNow
        };

        await _documentRepository.AddAsync(document, ct);
        return document.Id;
    }

    public async Task<(byte[] Content, string FileName, string ContentType)> DownloadAsync(Guid documentId, Guid userId,
        CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        if (!await _documentAccessService.HasAccessAsync(documentId, userId, DocumentAccess.ViewDocument, ct))
            throw new AccessDeniedException("Скачивание документа", document.ChatId, userId);

        try
        {
            var fileBytes = await File.ReadAllBytesAsync(document.FilePath, ct);
            return (fileBytes, document.FileName, document.FileType);
        }
        catch (Exception ex)
        {
            throw new BusinessRuleException($"Ошибка при чтении файла: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid documentId, Guid userId, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        var isAdmin = await _chatAccessService.IsAdminOfChat(document.ChatId, userId, ct);
        if (document.UploaderId != userId && !isAdmin)
            throw new AccessDeniedException("Удаление документа", document.ChatId, userId);

        try
        {
            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);
        }
        catch (Exception ex)
        {
            throw new BusinessRuleException($"Ошибка при удалении файла: {ex.Message}");
        }

        await _documentRepository.DeleteAsync(documentId, ct);
    }

    public async Task<IReadOnlyList<Document>> GetListByChatAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId, ct)
                   ?? throw new NotFoundException("Чат", chatId);

        if (!await _chatAccessService.HasAccessAsync(chatId, userId, ChatAccess.ReadMessages, ct))
            throw new AccessDeniedException("Просмотр документов", chatId, userId);

        var documents = await _documentRepository.GetByChatAsync(chatId, ct);
        var accessibleDocuments = new List<Document>();
        foreach (var document in documents)
        {
            if (await _documentAccessService.HasAccessAsync(document.Id, userId, DocumentAccess.ViewDocument, ct))
            {
                accessibleDocuments.Add(document);
            }
        }

        return accessibleDocuments.AsReadOnly();
    }

    public async Task<Document?> GetByIdAsync(Guid documentId, Guid userId, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);
        
        var chat = document.Chat ?? throw new NotFoundException("Чат", document.ChatId);

        if (!await _chatAccessService.HasAccessAsync(chat.Id, userId, ChatAccess.ReadMessages, ct))
        {
            throw new AccessDeniedException("Просмотр документов", chat.Id, userId);
        }

        if (await _documentAccessService.HasAccessAsync(document.Id, userId, DocumentAccess.ViewDocument, ct))
        {
            return document; // Возвращаем первый доступный документ
        }
        
        return null; // Если нет доступных документов
    }
}