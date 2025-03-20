using Messenger.Domain;

namespace Messenger.Application.Interfaces;

public interface IUserService
{
    Task<User> AuthenticateUserAsync(string username, string password);
    Task<Guid> RegisterUserAsync(string username, string email, string password, string corporateKey);
    Task<Boolean> UserExistsAsync(string username);
    Task<Boolean> ConfirmEmailAsync(string userId, string code);    
    Task<bool> ProcessRegistrationAsync(Guid userId, bool approve);

}
