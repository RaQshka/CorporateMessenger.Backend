using MediatR;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chats.Commands.CreateChat;

public class CreateChatCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public ChatTypes Type { get; set; }
    public Guid CreatorId { get; set; }
}