using MediatR;

namespace Messenger.Application.Messages.Commands.RemoveReaction;

public class RemoveReactionCommand : IRequest<Unit>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}