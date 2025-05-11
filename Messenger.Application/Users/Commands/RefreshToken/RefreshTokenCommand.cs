using MediatR;

namespace Messenger.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommand:IRequest<RefreshTokenResult>
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}