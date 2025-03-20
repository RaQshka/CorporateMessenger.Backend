using Messenger.Domain;

namespace Messenger.Application.Interfaces;

public interface IJwtProvider
{ 
    string GenerateToken(User user);
    
}