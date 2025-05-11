using FluentValidation;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Messages.Commands.AddReaction;

public class AddReactionCommandValidator : AbstractValidator<AddReactionCommand>
{
    private readonly IReactionService _reactionService;

    public AddReactionCommandValidator(IReactionService reactionService)
    {
        _reactionService = reactionService;

        RuleFor(x => x.MessageId).NotEmpty().WithMessage("Идентификатор сообщения обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
        RuleFor(x => x.ReactionType).IsInEnum().WithMessage("Недопустимый тип реакции");
        RuleFor(x => x)
            .MustAsync(NoExistingReaction)
            .WithMessage("Пользователь уже добавил реакцию к этому сообщению");
    }

    private async Task<bool> NoExistingReaction(AddReactionCommand command, CancellationToken cancellationToken)
    {
        var reactions = await _reactionService.GetByMessageAsync(command.MessageId, command.UserId, cancellationToken);
        return !reactions.Any(r => r.UserId == command.UserId);
    }
}