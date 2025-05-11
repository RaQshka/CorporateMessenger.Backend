using MediatR;
using Messenger.Application.Documents.Commands.DeleteDocument;
using Messenger.Application.Documents.Commands.GrantDocumentAccess;
using Messenger.Application.Documents.Commands.RevokeDocumentAccess;
using Messenger.Application.Documents.Commands.UploadDocument;
using Messenger.Application.Documents.Queries.DownloadDocument;
using Messenger.Application.Documents.Queries.GetDocumentAccessRules;
using Messenger.Application.Documents.Queries.GetDocuments;
using Messenger.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuditLogger _auditLogger;

    public DocumentsController(IMediator mediator, IAuditLogger auditLogger)
    {
        _mediator = mediator;
        _auditLogger = auditLogger;
    }

    [HttpPost("upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentCommand command)
    {
        var documentId = await _mediator.Send(command);
        await _auditLogger.LogAsync(command.UploaderId, "UploadDocument", "Document", documentId, "Документ загружен");
        return Ok(new { DocumentId = documentId });
    }

    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(Guid documentId, [FromQuery] Guid userId)
    {
        var query = new DownloadDocumentQuery { DocumentId = documentId, UserId = userId };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(userId, "DownloadDocument", "Document", documentId, "Документ скачан");
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpDelete("{documentId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(Guid documentId, [FromQuery] Guid userId)
    {
        var command = new DeleteDocumentCommand { DocumentId = documentId, UserId = userId };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(userId, "DeleteDocument", "Document", documentId, "Документ удален");
        return NoContent();
    }

    [HttpPost("{documentId}/access/grant")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GrantDocumentAccess(Guid documentId, [FromBody] GrantDocumentAccessCommand command)
    {
        if (command.DocumentId != documentId) return BadRequest("Document ID mismatch");
        await _mediator.Send(command);
        await _auditLogger.LogAsync(command.RoleId, "GrantDocumentAccess", "Document", documentId,
            "Доступ предоставлен");
        return NoContent();
    }

    [HttpPost("{documentId}/access/revoke")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeDocumentAccess(Guid documentId,
        [FromBody] RevokeDocumentAccessCommand command)
    {
        if (command.DocumentId != documentId) return BadRequest("Document ID mismatch");
        await _mediator.Send(command);
        await _auditLogger.LogAsync(command.RoleId, "RevokeDocumentAccess", "Document", documentId, "Доступ отозван");
        return NoContent();
    }

    [HttpGet("chat/{chatId}")]
    public async Task<IActionResult> GetDocuments(Guid chatId, [FromQuery] Guid userId)
    {
        var query = new GetDocumentsQuery { ChatId = chatId, UserId = userId };
        var documents = await _mediator.Send(query);
        await _auditLogger.LogAsync(userId, "GetDocuments", "Chat", chatId, "Документы получены");
        return Ok(documents);
    }

    [HttpGet("{documentId}/access")]
    public async Task<IActionResult> GetDocumentAccessRules(Guid documentId, [FromQuery] Guid userId)
    {
        var query = new GetDocumentAccessRulesQuery { DocumentId = documentId, UserId = userId };
        var rules = await _mediator.Send(query);
        await _auditLogger.LogAsync(userId, "GetDocumentAccessRules", "Document", documentId,
            "Правила доступа получены");
        return Ok(rules);
    }
}