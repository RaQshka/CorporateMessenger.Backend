using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Messages.Commands.DeleteMessage;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Unit>
{
    private readonly IMessageService _messageService;

    public DeleteMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageService.DeleteAsync(
            request.MessageId,
            request.UserId,
            cancellationToken);
        return Unit.Value;
    }
}