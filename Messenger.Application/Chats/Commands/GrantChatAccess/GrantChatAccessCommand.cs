using MediatR;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chats.Commands.GrantChatAccess;

public class GrantChatAccessCommand : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public Guid RoleId { get; set; }
    public ChatAccess Access { get; set; }
    public Guid InitiatorId { get; set; }
}