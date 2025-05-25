using MediatR;
namespace Messenger.Application.Messages.Commands.SendMessage;

public class SendMessageCommand : IRequest<Guid>
{
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public Guid? ReplyToMessageId { get; set; }
}