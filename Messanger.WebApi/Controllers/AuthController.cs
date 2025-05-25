using MediatR;
using Messenger.Application.Attributes;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Users.Commands.AssignRole;
using Messenger.Application.Users.Commands.ConfirmAccount;
using Messenger.Application.Users.Commands.ConfirmEmail;
using Messenger.Application.Users.Commands.DeleteUser;
using Messenger.Application.Users.Commands.LoginUser;
using Messenger.Application.Users.Commands.LogoutUser;
using Messenger.Application.Users.Commands.RefreshToken;
using Messenger.Application.Users.Commands.RegisterUser;
using Messenger.Application.Users.Commands.RemoveRole;
using Messenger.Application.Users.Queries.GetUnconfirmedUsers;
using Messenger.Application.Users.Queries.GetUserInfo;
using Messenger.Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[ApiController]
[AuditController]
[Route("api/auth")]
public class AuthController:BaseController
{
    private readonly IMediator _mediator;
    private readonly IAuditLogger _auditLogger;

    public AuthController(IMediator mediator, IAuditLogger auditLogger)
    {
        _mediator = mediator;
        _auditLogger = auditLogger;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [NoAudit]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        
        await _auditLogger.LogAsync(userId: result.UserId, 
                actionType: "Registration", 
                targetEntity: "User",
                targetId: result.UserId, 
                result.Message);
        
        return Ok(result);
    }

    /// <summary>
    /// Подтверждение Email
    /// </summary>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand { UserId = userId, Token = token });

        return result ? Ok("Email подтверждён.") : BadRequest("Ошибка подтверждения Email.");
    }

    /// <summary>
    /// Вход в систему
    /// </summary>
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (string.IsNullOrEmpty(result.Token))
            return Unauthorized(result.Message);
        
        return Ok(result);
    }

    /// <summary>
    /// Выход из системы
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutUserCommand());

        // Удаляем JWT-токен из Cookie
        Response.Cookies.Delete("AuthToken");

        return Ok("Вы успешно вышли из системы.");
    }

    /// <summary>
    /// Назначение роли пользователю (доступно только админам)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return Ok(new { message = result.Message });
    }
    /// <summary>
    /// Удаление роли у пользователя (доступно только админам)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole(RemoveRoleCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return Ok(new { message = result.Message });
    }
    
    /// <summary>
    /// Обновляет Access-токен и Refresh-токен
    /// </summary>
    /// <returns>AccessToken</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {    
        // Пытаемся получить значение "refreshToken" из куки запроса.
        // Если токен отсутствует, возвращаем статус "Unauthorized" с сообщением об ошибке.
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized(new { message = "Токен отсутствует" });
        }
        if (!Request.Cookies.TryGetValue("UserId", out var userId))
        {
            return Unauthorized(new { message = "Отсутствует Id пользователя в куки, запрет на обновление токена." });
        }

        var result = await _mediator.Send(new RefreshTokenCommand
        {
            RefreshToken = refreshToken, 
            UserId = Guid.Parse(userId)
        });

        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }

        return Ok(new { AccessToken = result.AccessToken });
    }
    
    /// <summary>
    /// Подтвердить аккканут пользователя, для админа
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("confirm-account")]
    public async Task<IActionResult> ConfirmAccount(ConfirmAccountCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success)
            return Unauthorized(new { message = result.Message });
        
        return Ok(new { message = result.Message });
    }    
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser(DeleteUserCommand command)
    {
        var result = await _mediator.Send(command);
        await _auditLogger.LogAsync(command.UserId, "DeleteUser", "User", command.UserId, "Пользователь был удален администратором.");

        return Ok();
    }

    /// <summary>
    /// Получить информацию о пользователе по Id
    /// </summary>
    /// <param name="command">GetUserInfoQuery</param>
    /// <returns>UserDetailsDto</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("user-info")]
    public async Task<IActionResult> GetUserInfo([FromQuery]GetUserInfoQuery command)
    {
        var result = await _mediator.Send(command);
        if (result == null)
        {
            return NoContent();
        }
        return Ok(result);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = UserId;
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(new GetUserInfoQuery{UserId = userId});
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-unconfirmed-users")]
    public async Task<IActionResult> GetUnconfirmedUsers()
    {
        var result = await _mediator.Send(new GetUnconfirmedUsersQuery());
        return Ok(result);
    }
    
}