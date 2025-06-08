    using MediatR;
    using Messenger.Application.Messages.Commands.AddReaction;
    using Messenger.Application.Messages.Commands.DeleteMessage;
    using Messenger.Application.Messages.Commands.EditMessage;
    using Messenger.Application.Messages.Commands.RemoveReaction;
    using Messenger.Application.Messages.Queries.GetMessages;
    using Messenger.Application.Messages.Queries.GetReactions;
    using Messenger.Application.Interfaces.Services;
    using Messenger.Application.Messages.Commands.SendMessage;
    using Messenger.Domain.Enums;
    using Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace Messenger.WebApi.Controllers;
    [ApiController]
    [Route("api/messages")]
    [Authorize]
    public class MessagesController : BaseController
    {
        private readonly IAuditLogger _auditLogger;

        public MessagesController(IAuditLogger auditLogger)
        {
            _auditLogger = auditLogger;
        }

        // Отправляет новое сообщение в чат
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            command.SenderId = UserId;
            var messageId = await Mediator.Send(command);
            await _auditLogger.LogAsync(UserId, "Отправка сообщения", "Сообщение", messageId, "Сообщение отправлено");
            return Ok(new { MessageId = messageId });
        }

        // Редактирует существующее сообщение
        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditMessageRequest request)
        {
            var command = new EditMessageCommand
            {
                MessageId = messageId,
                UserId = UserId,
                NewContent = request.NewContent
            };
            await Mediator.Send(command);
            await _auditLogger.LogAsync(UserId, "Редактирование сообщения", "Сообщение", messageId, "Сообщение отредактировано");
            return NoContent();
        }

        // Удаляет сообщение
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var command = new DeleteMessageCommand
            {
                MessageId = messageId,
                UserId = UserId
            };
            await Mediator.Send(command);
            await _auditLogger.LogAsync(UserId, "Удаление сообщения", "Сообщение", messageId, "Сообщение удалено");
            return NoContent();
        }

        // Добавляет реакцию к сообщению
        [HttpPost("{messageId}/reactions")]
        public async Task<IActionResult> AddReaction(Guid messageId, [FromBody] AddReactionRequest request)
        {
            if (!ReactionType.TryParse(request.ReactionType, out ReactionType reactionType))
                return NotFound();
            
            var command = new AddReactionCommand
            {
                MessageId = messageId,
                UserId = UserId,
                ReactionType = reactionType
            };
            
            await Mediator.Send(command);
            await _auditLogger.LogAsync(UserId, "Добавление реакции", "Реакция", messageId, "Реакция добавлена");
            return NoContent();
        }

        // Удаляет реакцию с сообщения
        [HttpDelete("{messageId}/reactions")]
        public async Task<IActionResult> RemoveReaction(Guid messageId)
        {
            var command = new RemoveReactionCommand
            {
                MessageId = messageId,
                UserId = UserId
            };
            await Mediator.Send(command);
            await _auditLogger.LogAsync(UserId, "Удаление реакции", "Реакция", messageId, "Реакция удалена");
            return NoContent();
        }

        // Получает список сообщений в чате с пагинацией
        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetMessages(Guid chatId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var query = new GetMessagesQuery
            {
                ChatId = chatId,
                UserId = UserId,
                Skip = skip,
                Take = take
            };
            var messages = await Mediator.Send(query);
            await _auditLogger.LogAsync(UserId, "Получение сообщений", "Чат", chatId, "Сообщения получены");
            return Ok(messages);
        }

        // Получает список реакций на сообщение
        [HttpGet("{messageId}/reactions")]
        public async Task<IActionResult> GetReactions(Guid messageId)
        {
            var query = new GetReactionsQuery
            {
                MessageId = messageId,
                UserId = UserId
            };
            var reactions = await Mediator.Send(query);
            await _auditLogger.LogAsync(UserId, "Получение реакций", "Сообщение", messageId, "Реакции получены");
            return Ok(reactions);
        }
    }

    // DTO для минимизации передаваемых данных
    public class EditMessageRequest
    {
        public string NewContent { get; set; }
    }

    public class AddReactionRequest
    {
        public string ReactionType { get; set; }
    }