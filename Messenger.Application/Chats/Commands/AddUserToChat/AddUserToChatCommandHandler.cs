using MediatR;
using Messenger.Application.Interfaces;
using Messenger.Domain;

namespace Messenger.Application.Chats.Commands.AddUserToChat;

public class AddUserToChatCommandHandler : IRequestHandler<AddUserToChatCommand, bool>
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserAccessService _userAccessService;

    public AddUserToChatCommandHandler(IChatRepository chatRepository, IUserAccessService userAccessService)
    {
        _chatRepository = chatRepository;
        _userAccessService = userAccessService;
    }

    public async Task<bool> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
    {
        // Допустим, проверяем, имеет ли пользователь, выполняющий запрос, право добавлять участников.
        // Здесь можно использовать другую логику, например, только создатель или администраторы могут добавлять.
        var chat = await _chatRepository.GetChatByIdAsync(request.ChatId, cancellationToken);
        if (chat == null)
            return false;

        // Проверяем, что запрашивающий является создателем или администратором
        if (chat.CreatedBy != request.RequestedBy && !chat.ChatParticipants.Any(cp => cp.UserId == request.RequestedBy && cp.IsAdmin))
            throw new UnauthorizedAccessException("У вас нет прав добавлять пользователей в этот чат.");

        // Проверяем, может ли добавляемый пользователь присоединиться (по логике доступа, если нужно)
        var canJoin = await _userAccessService.CanJoinChatAsync(request.ChatId, request.UserIdToAdd);
        if (!canJoin)
            throw new UnauthorizedAccessException("Пользователь не может быть добавлен в этот чат.");

        // Добавляем пользователя в чат
        // Здесь можно либо напрямую обновить коллекцию chat.ChatParticipants, либо использовать репозиторий.
        // Предположим, что IChatRepository имеет метод UpdateChatAsync
        chat.ChatParticipants.Add(new ChatParticipant
        {
            ChatId = request.ChatId,
            UserId = request.UserIdToAdd,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = false
        });

        await _chatRepository.UpdateChatAsync(chat, cancellationToken);
        return true;
    }
}