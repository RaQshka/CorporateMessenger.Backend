using MediatR;
using Messenger.Application.Common.Exceptions;
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
        if(await ExistingReaction(request, cancellationToken))
        {
            await _reactionService.RemoveAsync(
                request.MessageId,
                request.UserId,
                cancellationToken);
            //throw new BusinessRuleException("Пользователь уже добавил реакцию к этому сообщению");
        }
        await _reactionService.AddAsync(
            request.MessageId,
            request.UserId,
            request.ReactionType,
            cancellationToken);
        return Unit.Value;
        
        
    }
    private async Task<bool> ExistingReaction(AddReactionCommand command, CancellationToken cancellationToken)
    {
        var reactions = await _reactionService.GetByMessageAsync(command.MessageId, command.UserId, cancellationToken);
        return reactions.Any(r => r.UserId == command.UserId);
    }
}
