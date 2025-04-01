using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly MessengerDbContext _context;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(MessengerDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> GenerateRefreshToken(Guid userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenLifetimeDays"]));

        var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        if (existingToken != null)
        {
            existingToken.Token = refreshToken;
            existingToken.ExpiresAt = expiresAt;
        }
        else
        {
            await _context.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = expiresAt
            });
        }

        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<bool> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        return storedToken != null &&
               storedToken.Token == refreshToken &&
               storedToken.ExpiresAt > DateTime.UtcNow;
    }

    public async Task RevokeRefreshToken(Guid userId)
    {
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        if (storedToken != null)
        {
            _context.RefreshTokens.Remove(storedToken);
            await _context.SaveChangesAsync();
        }
    }
}
