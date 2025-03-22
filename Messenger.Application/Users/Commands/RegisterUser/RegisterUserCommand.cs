using MediatR;

namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegisterUserCommand:IRequest<RegistrationResult>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string CorporateKey { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}