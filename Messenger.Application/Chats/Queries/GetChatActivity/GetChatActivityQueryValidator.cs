using FluentValidation;

namespace Messenger.Application.Chats.Queries.GetChatActivity;

public class GetChatActivityQueryValidator : AbstractValidator<GetChatActivityQuery>
{
    public GetChatActivityQueryValidator()
    {
        RuleFor(x=>x.ChatId).NotEmpty().WithMessage("ChatId cannot be empty");
        RuleFor(x=>x.UserId).NotEmpty().WithMessage("UserId cannot be empty");
        
    }
}