using MediatR;

namespace Messenger.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
}