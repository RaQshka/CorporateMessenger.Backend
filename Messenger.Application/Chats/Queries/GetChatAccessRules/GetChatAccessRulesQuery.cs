using MediatR;
using Messenger.Application.Chats.Queries.Shared;

namespace Messenger.Application.Chats.Queries.GetChatAccessRules;


// Запрос правил доступа чата
public class GetChatAccessRulesQuery : IRequest<List<ChatAccessRuleDto>>
{
    public Guid ChatId { get; set; }
    public Guid InitiatorId { get; set; }
}