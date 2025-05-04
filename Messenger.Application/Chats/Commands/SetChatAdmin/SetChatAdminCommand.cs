using MediatR;

namespace Messenger.Application.Chats.Commands.SetChatAdmin;

public class SetChatAdminCommand : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public Guid InitiatorId { get; set; }
    public bool IsAdmin { get; set; }
}