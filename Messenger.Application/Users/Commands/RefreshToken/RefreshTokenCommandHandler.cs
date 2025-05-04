using MediatR;
using Messenger.Application.Interfaces;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler:IRequestHandler<RefreshTokenCommand,RefreshTokenResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenCommandHandler(
        UserManager<User> userManager,
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshTokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _refreshTokenService = refreshTokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return new RefreshTokenResult { Success = false, Message = "Пользователь не найден" };
        }
        
        var isValid = await _refreshTokenService.ValidateRefreshToken(request.UserId, request.RefreshToken);
        if (!isValid)
        {
            return new RefreshTokenResult { Success = false, Message = "Невалидный refresh token" };
        }
        
        var newAccessToken = await _jwtProvider.GenerateToken(user);
        var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id);

        var response = _httpContextAccessor.HttpContext.Response;
        response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return new RefreshTokenResult
        {
            Success = true,
            AccessToken = newAccessToken
        };
    }
}