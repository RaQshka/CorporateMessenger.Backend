using MediatR;
using Messenger.Application.Interfaces.Services;


namespace Messenger.Application.Messages.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IMessageService _messageService;

    public SendMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        return await _messageService.SendAsync(
            request.ChatId,
            request.SenderId,
            request.Content,
            request.ReplyToMessageId,
            cancellationToken);
    }
}