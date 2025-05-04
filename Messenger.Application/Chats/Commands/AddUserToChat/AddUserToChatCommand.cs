using MediatR;

namespace Messenger.Application.Chats.Commands.AddUserToChat;

public class AddUserToChatCommand : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public Guid InitiatorId { get; set; }
}