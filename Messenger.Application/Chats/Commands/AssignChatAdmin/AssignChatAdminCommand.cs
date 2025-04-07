using MediatR;

namespace Messenger.Application.Chats.Commands.AssignChatAdmin;

public class AssignChatAdminCommand : IRequest<bool>
{
    public Guid ChatId { get; set; }
    public Guid UserIdToAssign { get; set; }
    public Guid RequestedBy { get; set; }
}
