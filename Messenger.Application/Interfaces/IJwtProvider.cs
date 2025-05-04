using Messenger.Domain;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface IJwtProvider
{ 
    Task<string> GenerateToken(User user);
    
}