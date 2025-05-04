using MediatR;
using Messenger.Application.Chats.Queries.Shared;

namespace Messenger.Application.Chats.Queries.GetUserChats;

// Запрос списка чатов пользователя
public class GetUserChatsQuery : IRequest<List<UserChatDto>>
{
    public Guid UserId { get; set; }
}