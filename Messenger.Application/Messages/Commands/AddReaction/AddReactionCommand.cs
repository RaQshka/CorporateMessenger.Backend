using MediatR;
using Messenger.Domain.Enums;

namespace Messenger.Application.Messages.Commands.AddReaction;
public class AddReactionCommand : IRequest<Unit>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
}