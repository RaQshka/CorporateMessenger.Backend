using MediatR;
using Messenger.Application.Chats.Queries.Shared;

namespace Messenger.Application.Chats.Queries.GetChatParticipants;

// Запрос списка участников чата
public class GetChatParticipantsQuery : IRequest<List<ChatParticipantDto>>
{
    public Guid ChatId { get; set; }
    public Guid InitiatorId { get; set; }
}