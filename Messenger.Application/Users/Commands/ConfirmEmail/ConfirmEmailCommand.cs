using MediatR;
using Messenger.Application.Users.Commands.LoginUser;

namespace Messenger.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}