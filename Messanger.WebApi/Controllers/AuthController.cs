﻿using MediatR;
using Messenger.Application.Users.Commands.AssignRole;
using Messenger.Application.Users.Commands.ConfirmEmail;
using Messenger.Application.Users.Commands.LoginUser;
using Messenger.Application.Users.Commands.LogoutUser;
using Messenger.Application.Users.Commands.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.WebApi.Models.Auth;

namespace Messenger.WebApi.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController:BaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
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

        // Записываем токен в HttpOnly Cookie
        Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Используется только при HTTPS
            SameSite = SameSiteMode.Strict
        });

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
        return Ok(new { message = result });
    }
}