using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Services;

public interface IJwtProvider
{ 
    Task<string> GenerateToken(User user);
    
}