using MediatR;

namespace Messenger.Application.Chats.Commands.AssignChatAdmin;

public class AssignChatAdminCommandHandler : IRequestHandler<AssignChatAdminCommand, bool>
{
    private readonly IChatRepository _chatRepository;

    public AssignChatAdminCommandHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<bool> Handle(AssignChatAdminCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetChatByIdAsync(request.ChatId, cancellationToken);
        if (chat == null)
            return false;

        // Проверяем, что запрашивающий имеет право назначать администраторов (например, создатель чата)
        if (chat.CreatedBy != request.RequestedBy)
            throw new UnauthorizedAccessException("Только создатель чата может назначать администраторов.");

        var participant = chat.ChatParticipants.FirstOrDefault(cp => cp.UserId == request.UserIdToAssign);
        if (participant == null)
            throw new InvalidOperationException("Пользователь не является участником чата.");

        participant.IsAdmin = true;
        await _chatRepository.UpdateChatAsync(chat, cancellationToken);
        return true;
    }
}