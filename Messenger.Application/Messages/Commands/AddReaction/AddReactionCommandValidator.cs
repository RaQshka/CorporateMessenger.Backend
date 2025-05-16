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

    }

}