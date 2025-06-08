using MediatR;
using Messenger.Application.Chats.Commands.AddUserToChat;
using Messenger.Application.Chats.Commands.CreateChat;
using Messenger.Application.Chats.Commands.DeleteChat;
using Messenger.Application.Chats.Commands.GrantChatAccess;
using Messenger.Application.Chats.Commands.RemoveUserFromChat;
using Messenger.Application.Chats.Commands.RenameChat;
using Messenger.Application.Chats.Commands.RevokeChatAccess;
using Messenger.Application.Chats.Commands.SetChatAdmin;
using Messenger.Application.Chats.Queries.GetChatAccessRules;
using Messenger.Application.Chats.Queries.GetChatActivity;
using Messenger.Application.Chats.Queries.GetChatInfo;
using Messenger.Application.Chats.Queries.GetChatParticipants;
using Messenger.Application.Chats.Queries.GetUserChatAccess;
using Messenger.Application.Chats.Queries.GetUserChats;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Enums;
using Messenger.WebApi.Models.ChatDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize] // Требуется аутентификация для всех эндпоинтов
public class ChatController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IAuditLogger _auditLogger;

    public ChatController(IMediator mediator, IAuditLogger auditLogger)
    {
        _mediator = mediator;
        _auditLogger = auditLogger;
    }

    /// <summary>
    /// Получает список всех чатов пользователя
    /// </summary>
    /// <returns>Список чатов</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserChats()
    {
        var query = new GetUserChatsQuery
        {
            UserId = UserId
        };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "GetUserChats", "Chat", UserId,
            "Список чатов пользователя успешно получен");
        return Ok(result);
    }

    /// <summary>
    /// Создает новый чат
    /// </summary>
    /// <param name="dto">Данные для создания чата</param>
    /// <returns>ID созданного чата</returns>
    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatDto dto)
    {
        var command = new CreateChatCommand
        {
            Name = dto.Name,
            Type = dto.Type,
            CreatorId = UserId // Предполагается метод расширения для получения ID пользователя
        };
        var chatId = await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "CreateChat", "Chat", chatId, "Чат успешно создан");
        return Ok(new { ChatId = chatId });
    }

    /// <summary>
    /// Удаляет чат
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Статус удаления</returns>
    [HttpDelete("{chatId}")]
    public async Task<IActionResult> DeleteChat(Guid chatId)
    {
        var command = new DeleteChatCommand
        {
            ChatId = chatId,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "DeleteChat", "Chat", chatId, "Чат успешно удален");
        return Ok(new { message = "Чат успешно удален" });
    }

    /// <summary>
    /// Переименовывает чат
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="dto">Новое имя чата</param>
    /// <returns>Сообщение об успехе</returns>
    [HttpPut("{chatId}/rename")]
    public async Task<IActionResult> RenameChat(Guid chatId, [FromBody] RenameChatDto dto)
    {
        var command = new RenameChatCommand
        {
            ChatId = chatId,
            NewName = dto.NewName,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "RenameChat", "Chat", chatId, "Чат успешно переименован");
        return Ok(new { message = "Чат успешно переименован" });
    }

    /// <summary>
    /// Добавляет пользователя в чат
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="dto">ID пользователя</param>
    /// <returns>Сообщение об успехе</returns>
    [HttpPost("{chatId}/users")]
    public async Task<IActionResult> AddUserToChat(Guid chatId, [FromBody] AddUserDto dto)
    {
        var command = new AddUserToChatCommand
        {
            ChatId = chatId,
            UserId = dto.UserId,
            UserEmail = dto.UserEmail,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "AddUserToChat", "Chat", chatId,
            "Пользователь успешно добавлен в чат");
        return Ok(new { message = "Пользователь успешно добавлен в чат" });
    }

    /// <summary>
    /// Удаляет пользователя из чата
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Статус удаления</returns>
    [HttpDelete("{chatId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromChat(Guid chatId, Guid userId)
    {
        var command = new RemoveUserFromChatCommand
        {
            ChatId = chatId,
            UserId = userId,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "RemoveUserFromChat", "Chat", chatId,
            "Пользователь успешно удален из чата");
        return NoContent();
    }

    /// <summary>
    /// Назначает или снимает статус администратора
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="dto">Статус администратора</param>
    /// <returns>Сообщение об успехе</returns>
    [HttpPut("{chatId}/users/{userId}/admin")]
    public async Task<IActionResult> SetChatAdmin(Guid chatId, Guid userId, [FromBody] SetAdminDto dto)
    {
        var command = new SetChatAdminCommand
        {
            ChatId = chatId,
            UserId = userId,
            IsAdmin = dto.IsAdmin,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "SetChatAdmin", "Chat", chatId,
            $"Статус администратора изменен на {dto.IsAdmin}");
        return Ok(new { message = "Статус администратора успешно обновлен" });
    }

    /// <summary>
    /// Предоставляет доступ роли к чату
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="dto">Данные для предоставления доступа</param>
    /// <returns>Сообщение об успехе</returns>
    [HttpPost("{chatId}/access/grant")]
    public async Task<IActionResult> GrantChatAccess(Guid chatId, [FromBody] AccessDto dto)
    {
        var command = new GrantChatAccessCommand
        {
            ChatId = chatId,
            RoleId = dto.RoleId,
            Access = dto.Access,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "GrantChatAccess", "Chat", chatId, "Доступ успешно предоставлен");
        return Ok(new { message = "Доступ успешно предоставлен" });
    }

    /// <summary>
    /// Отзывает доступ роли к чату
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="dto">Данные для отзыва доступа</param>
    /// <returns>Сообщение об успехе</returns>
    [HttpPost("{chatId}/access/revoke")]
    public async Task<IActionResult> RevokeChatAccess(Guid chatId, [FromBody] AccessDto dto)
    {
        var command = new RevokeChatAccessCommand
        {
            ChatId = chatId,
            RoleId = dto.RoleId,
            Access = dto.Access,
            InitiatorId = UserId
        };
        await _mediator.Send(command);
        await _auditLogger.LogAsync(UserId, "RevokeChatAccess", "Chat", chatId, "Доступ успешно отозван");
        return Ok(new { message = "Доступ успешно отозван" });
    }

    /// <summary>
    /// Получает информацию о чате
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Информация о чате</returns>
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChatInfo(Guid chatId)
    {
        var query = new GetChatInfoQuery
        {
            ChatId = chatId,
            InitiatorId = UserId
        };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "GetChatInfo", "Chat", chatId,
            "Информация о чате успешно получена");
        return Ok(result);
    }

    /// <summary>
    /// Получает список участников чата
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Список участников</returns>
    [HttpGet("{chatId}/users")]
    public async Task<IActionResult> GetChatParticipants(Guid chatId)
    {
        var query = new GetChatParticipantsQuery
        {
            ChatId = chatId,
            InitiatorId = UserId
        };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "GetChatParticipants", "Chat", chatId,
            "Список участников чата успешно получен");
        return Ok(result);
    }

    /// <summary>
    /// Получает правила доступа чата
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Список правил доступа</returns>
    [HttpGet("{chatId}/access")]
    public async Task<IActionResult> GetChatAccessRules(Guid chatId)
    {
        var query = new GetChatAccessRulesQuery
        {
            ChatId = chatId,
            InitiatorId = UserId
        };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "GetChatAccessRules", "Chat", chatId,
            "Правила доступа чата успешно получены");
        return Ok(result);
    }
    [HttpGet("{chatId}/access/{userId}")]
    public async Task<IActionResult> GetUserChatAccessRules(Guid chatId, Guid userId)
    {
        var query = new GetUserChatAccessQuery
        {
            ChatId = chatId,
            InitiatorId = userId
        };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "GetUserChatAccessQuery", "Chat", chatId,
            "Правила доступа пользователя успешно получены");
        return Ok(result);
    }
    
    [HttpGet("{chatId}/activity")]
    public async Task<IActionResult> GetChatActivity(Guid chatId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var query = new GetChatActivityQuery
        {
            ChatId = chatId,
            UserId = UserId,
            Skip = skip,
            Take = take
        };
        var result = await _mediator.Send(query);
        await _auditLogger.LogAsync(UserId, "GetChatActivity", "Chat", chatId, "Активность чата получена");
        return Ok(result);
    }
    
    
    
}