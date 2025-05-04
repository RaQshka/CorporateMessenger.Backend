using MediatR;

namespace Messenger.Application.Chats.Commands.DeleteChat;

public class DeleteChatCommand : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public Guid InitiatorId { get; set; }
}
