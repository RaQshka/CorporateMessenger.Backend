using Messenger.Domain;

namespace Messenger.Application.Interfaces;

public interface IJwtProvider
{ 
    Task<string> GenerateToken(User user);
    
}