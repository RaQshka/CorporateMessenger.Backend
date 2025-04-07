using MediatR;

namespace Messenger.Application.Chats.Commands.AddUserToChat;

public class AddUserToChatCommand : IRequest<bool>
{
    public Guid ChatId { get; set; }
    public Guid UserIdToAdd { get; set; }
    public Guid RequestedBy { get; set; } // Кто запрашивает добавление (для проверки прав)
}