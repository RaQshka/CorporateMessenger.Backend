using MediatR;

namespace Messenger.Application.Users.Commands.AssignRole;

public class AssignRoleCommand:IRequest<AssignRoleResult>
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}