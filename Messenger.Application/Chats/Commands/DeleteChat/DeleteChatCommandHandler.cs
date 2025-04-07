using MediatR;

namespace Messenger.Application.Chats.Commands.DeleteChat;

public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, bool>
{
    private readonly IChatRepository _chatRepository;

    public DeleteChatCommandHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<bool> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        await _chatRepository.DeleteChatAsync(request.ChatId, cancellationToken);
        return true;
    }
}