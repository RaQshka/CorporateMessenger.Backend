namespace Messenger.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshToken(Guid userId);
    Task<bool> ValidateRefreshToken(Guid userId, string refreshToken);
    Task RevokeRefreshToken(Guid userId);
}