/*using MediatR;
using Messenger.Application.Messages.Commands.Shared;

namespace Messenger.Application.Messages.Commands.SendMessage;

public class SendMessageCommand : IRequest<MessageDto>
{
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
}*/