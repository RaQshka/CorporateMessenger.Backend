using MediatR;
using Messenger.Application.Documents.Commands.DeleteDocument;
using Messenger.Application.Documents.Commands.GrantDocumentAccess;
using Messenger.Application.Documents.Commands.RevokeDocumentAccess;
using Messenger.Application.Documents.Commands.UploadDocument;
using Messenger.Application.Documents.Queries.DownloadDocument;
using Messenger.Application.Documents.Queries.GetDocumentAccessRules;
using Messenger.Application.Documents.Queries.GetDocuments;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Enums;
using Messenger.WebApi.Hubs;
using Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.WebApi.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : BaseController
{
    private readonly IAuditLogger _auditLogger;

    public DocumentsController(IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    // Загружает новый документ в чат
    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentCommand command)
    {
        command.UploaderId = UserId;
        var documentId = await Mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "Загрузка документа", "Документ", documentId, "Документ загружен");
        
        var hubContext = HttpContext.RequestServices.GetService<IHubContext<ChatHub>>();
        await hubContext.Clients.All.SendAsync("ReceiveDocument", command.ChatId, documentId);
        
        return Ok(new { DocumentId = documentId });
    }

    // Скачивает документ
    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(Guid documentId)
    {
        var query = new DownloadDocumentQuery
        {
            DocumentId = documentId,
            UserId = UserId
        };
        var result = await Mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "Скачивание документа", "Документ", documentId, "Документ скачан");
        return File(result.Content, result.ContentType, result.FileName);
    }

    // Удаляет документ
    [HttpDelete("{documentId}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var command = new DeleteDocumentCommand
        {
            DocumentId = documentId,
            UserId = UserId
        };
        await Mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "Удаление документа", "Документ", documentId, "Документ удален");
        return NoContent();
    }

    // Предоставляет доступ к документу для роли
    [HttpPost("{documentId}/access/grant")]
    public async Task<IActionResult> GrantDocumentAccess(Guid documentId, [FromBody] DocumentAccessRequest request)
    {
        var command = new GrantDocumentAccessCommand
        {
            DocumentId = documentId,
            InitiatorId = UserId,
            RoleId = request.RoleId,
            AccessFlag = request.AccessFlag
        };
        await Mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "Предоставление доступа", "Документ", documentId, "Доступ предоставлен");
        return NoContent();
    }

    // Отзывает доступ к документу у роли
    [HttpPost("{documentId}/access/revoke")]
    public async Task<IActionResult> RevokeDocumentAccess(Guid documentId, [FromBody] DocumentAccessRequest request)
    {
        var command = new RevokeDocumentAccessCommand
        {
            DocumentId = documentId,
            InitiatorId = UserId,
            RoleId = request.RoleId,
            AccessFlag = request.AccessFlag
        };
        await Mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "Отзыв доступа", "Документ", documentId, "Доступ отозван");
        return NoContent();
    }

    // Получает список документов в чате
    [HttpGet("chat/{chatId}")]
    public async Task<IActionResult> GetDocuments(Guid chatId)
    {
        var query = new GetDocumentsQuery
        {
            ChatId = chatId,
            UserId = UserId
        };
        var documents = await Mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "Получение документов", "Чат", chatId, "Документы получены");
        return Ok(documents);
    }

    // Получает правила доступа для документа
    [HttpGet("{documentId}/access")]
    public async Task<IActionResult> GetDocumentAccessRules(Guid documentId)
    {
        var query = new GetDocumentAccessRulesQuery
        {
            DocumentId = documentId,
            UserId = UserId
        };
        var rules = await Mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "Получение правил доступа", "Документ", documentId,
            "Правила доступа получены");
        return Ok(rules);
    }
}
