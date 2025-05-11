using MediatR;

namespace Messenger.Application.Messages.Commands.DeleteMessage;

public class DeleteMessageCommand : IRequest<Unit>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}