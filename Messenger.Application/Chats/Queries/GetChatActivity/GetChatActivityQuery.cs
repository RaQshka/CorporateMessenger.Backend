using MediatR;
using Messenger.Application.Chats.Queries.Shared;

namespace Messenger.Application.Chats.Queries.GetChatActivity;

public class GetChatActivityQuery : IRequest<IReadOnlyList<ChatActivityDto>>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; } 
}