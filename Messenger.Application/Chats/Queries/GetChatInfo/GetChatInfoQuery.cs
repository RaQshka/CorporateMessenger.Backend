using MediatR;
using Messenger.Application.Chats.Queries.Shared;

namespace Messenger.Application.Chats.Queries.GetChatInfo;

public class GetChatInfoQuery : IRequest<ChatInfoDto>
{
    public Guid ChatId { get; set; }
    public Guid InitiatorId { get; set; }
}