using System.Security.Claims;
using System.Text;
using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResult>
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LoginUserCommandHandler(
        SignInManager<User> signInManager, 
        UserManager<User> userManager, 
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshTokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _refreshTokenService = refreshTokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return new LoginResult { Message = "Неверные учетные данные." };

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (!result.Succeeded)
            return new LoginResult { Message = "Ошибка входа." };

        if (!user.EmailConfirmed)
            return new LoginResult { Message = "Email не подтверждён." };

        if (user.RegistrationStatus != "Approved")
            return new LoginResult { Message = "Регистрация не одобрена администратором." };

        var accessToken = await AddJwtCookies(user);
        
        user.LastLogin = DateTime.Now;
        await _userManager.UpdateAsync(user);
        
        return new LoginResult
        {
            UserId = user.Id,
            Token = accessToken,
            Message = "Успешный вход."
        };
    }
    
    /// Функция генерации и добавления токенов в куки
    private async Task<string> AddJwtCookies(User user)
    {
        var accessToken = await _jwtProvider.GenerateToken(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id);

        var response = _httpContextAccessor.HttpContext.Response;
        // Записываем токен в HttpOnly Cookie
        response.Cookies.Append("AuthToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Используется только при HTTPS
            SameSite = SameSiteMode.Strict,
        });
        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1)
        });
        response.Cookies.Append("UserId", user.Id.ToString(), new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1)
        });

        return accessToken;
    }

}