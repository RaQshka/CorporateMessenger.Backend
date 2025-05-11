using MediatR;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Messages.Commands.AddReaction;
using Messenger.Application.Messages.Commands.DeleteMessage;
using Messenger.Application.Messages.Commands.EditMessage;
using Messenger.Application.Messages.Commands.RemoveReaction;
using Messenger.Application.Messages.Queries.GetMessages;
using Messenger.Application.Messages.Queries.GetReactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuditLogger _auditLogger;

    public MessagesController(IMediator mediator, IAuditLogger auditLogger)
    {
        _mediator = mediator;
        _auditLogger = auditLogger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
    {
        var messageId = await _mediator.Send(command);
        await _auditLogger.LogAsync(command.SenderId, "SendMessage", "Message", messageId, "Сообщение отправлено");
        return Ok(new { MessageId = messageId });
    }

    [HttpPut("{messageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditMessageCommand command)
    {
        if (command.MessageId != messageId) return BadRequest("Message ID mismatch");
        await _mediator.Send(command);
        await _auditLogger.LogAsync(command.UserId, "EditMessage", "Message", messageId, "Сообщение отредактировано");
        return NoContent();
    }

    [HttpDelete("{messageId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(Guid messageId, [FromQuery] Guid userId)
    {
        var command = new DeleteMessageCommand { MessageId = messageId, UserId = userId };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(userId, "DeleteMessage", "Message", messageId, "Сообщение удалено");
        return NoContent();
    }

    [HttpPost("{messageId}/reactions")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReaction(Guid messageId, [FromBody] AddReactionCommand command)
    {
        if (command.MessageId != messageId) return BadRequest("Message ID mismatch");
        await _mediator.Send(command);
        await _auditLogger.LogAsync(command.UserId, "AddReaction", "Reaction", messageId, "Реакция добавлена");
        return NoContent();
    }

    [HttpDelete("{messageId}/reactions")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveReaction(Guid messageId, [FromQuery] Guid userId)
    {
        var command = new RemoveReactionCommand { MessageId = messageId, UserId = userId };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(userId, "RemoveReaction", "Reaction", messageId, "Реакция удалена");
        return NoContent();
    }

    [HttpGet("chat/{chatId}")]
    public async Task<IActionResult> GetMessages(Guid chatId, [FromQuery] Guid userId, [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var query = new GetMessagesQuery { ChatId = chatId, UserId = userId, Skip = skip, Take = take };
        var messages = await _mediator.Send(query);
        await _auditLogger.LogAsync(userId, "GetMessages", "Chat", chatId, "Сообщения получены");
        return Ok(messages);
    }

    [HttpGet("{messageId}/reactions")]
    public async Task<IActionResult> GetReactions(Guid messageId, [FromQuery] Guid userId)
    {
        var query = new GetReactionsQuery { MessageId = messageId, UserId = userId };
        var reactions = await _mediator.Send(query);
        await _auditLogger.LogAsync(userId, "GetReactions", "Message", messageId, "Реакции получены");
        return Ok(reactions);
    }
}