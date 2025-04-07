using MediatR;

namespace Messenger.Application.Chats.Commands.RemoveUserFromChat;

public class RemoveUserFromChatCommandHandler : IRequestHandler<RemoveUserFromChatCommand, bool>
{
    private readonly IChatRepository _chatRepository;

    public RemoveUserFromChatCommandHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<bool> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetChatByIdAsync(request.ChatId, cancellationToken);
        if (chat == null)
            return false;

        // Здесь также проверяем, что запрашивающий имеет право удалять участника (например, только администратор или создатель)
        if (chat.CreatedBy != request.RequestedBy && !chat.ChatParticipants.Any(cp => cp.UserId == request.RequestedBy && cp.IsAdmin))
            throw new UnauthorizedAccessException("У вас нет прав удалять участников из этого чата.");

        var participant = chat.ChatParticipants.FirstOrDefault(cp => cp.UserId == request.UserIdToRemove);
        if (participant == null)
            return false;

        chat.ChatParticipants.Remove(participant);
        await _chatRepository.UpdateChatAsync(chat, cancellationToken);
        return true;
    }
}