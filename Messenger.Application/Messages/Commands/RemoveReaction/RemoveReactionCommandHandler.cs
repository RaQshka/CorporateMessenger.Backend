using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Messages.Commands.RemoveReaction;

public class RemoveReactionCommandHandler : IRequestHandler<RemoveReactionCommand,Unit>
{
    private readonly IReactionService _reactionService;

    public RemoveReactionCommandHandler(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    public async Task<Unit> Handle(RemoveReactionCommand request, CancellationToken cancellationToken)
    {
        await _reactionService.RemoveAsync(
            request.MessageId,
            request.UserId,
            cancellationToken);
        return Unit.Value;
    }
}