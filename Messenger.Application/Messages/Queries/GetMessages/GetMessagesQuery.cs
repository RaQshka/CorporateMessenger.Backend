using MediatR;
using Messenger.Application.Messages.Queries.Shared;

namespace Messenger.Application.Messages.Queries.GetMessages;


public class GetMessagesQuery : IRequest<IReadOnlyList<MessageDto>>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}