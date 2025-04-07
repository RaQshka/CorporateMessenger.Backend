using MediatR;

namespace Messenger.Application.Chats.Commands.RemoveUserFromChat;

public class RemoveUserFromChatCommand : IRequest<bool>
{
    public Guid ChatId { get; set; }
    public Guid UserIdToRemove { get; set; }
    public Guid RequestedBy { get; set; }
}
