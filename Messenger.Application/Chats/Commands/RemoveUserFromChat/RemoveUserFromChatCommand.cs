using MediatR;

namespace Messenger.Application.Chats.Commands.RemoveUserFromChat;

public class RemoveUserFromChatCommand : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public Guid InitiatorId { get; set; }
}