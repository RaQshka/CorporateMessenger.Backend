using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Messages.Commands.EditMessage.Messenger.Application.Messages.Commands;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Unit>
{
    private readonly IMessageService _messageService;

    public EditMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<Unit> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageService.UpdateAsync(
            request.MessageId,
            request.UserId,
            request.NewContent,
            cancellationToken);
        return Unit.Value;
    }
}