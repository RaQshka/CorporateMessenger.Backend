using MediatR;

namespace Messenger.Application.Chats.Commands.RenameChat;


public class RenameChatCommand : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public Guid InitiatorId { get; set; }
    public string NewName { get; set; } = string.Empty;
}