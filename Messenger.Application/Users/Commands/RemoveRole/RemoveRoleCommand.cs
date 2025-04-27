using MediatR;

namespace Messenger.Application.Users.Commands.RemoveRole;

public class RemoveRoleCommand :IRequest<RemoveRoleResult>
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}