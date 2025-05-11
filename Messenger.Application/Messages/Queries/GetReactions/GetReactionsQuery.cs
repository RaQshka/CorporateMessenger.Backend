using MediatR;
using Messenger.Application.Messages.Queries.Shared;

namespace Messenger.Application.Messages.Queries.GetReactions;

public class GetReactionsQuery : IRequest<IReadOnlyList<MessageReactionDto>>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}