using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Messages.Commands.AddReaction;


public class AddReactionCommandHandler : IRequestHandler<AddReactionCommand, Unit>
{
    private readonly IReactionService _reactionService;

    public AddReactionCommandHandler(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    public async Task<Unit> Handle(AddReactionCommand request, CancellationToken cancellationToken)
    {
        await _reactionService.AddAsync(
            request.MessageId,
            request.UserId,
            request.ReactionType,
            cancellationToken);
        return Unit.Value;
    }
}
