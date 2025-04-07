using MediatR;

namespace Messenger.Application.Chats.Commands.DeleteChat;

public class DeleteChatCommand : IRequest<bool>
{
    public Guid ChatId { get; set; }
}
