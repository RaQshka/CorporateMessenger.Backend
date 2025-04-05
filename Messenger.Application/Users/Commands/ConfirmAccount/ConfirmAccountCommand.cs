using MediatR;

namespace Messenger.Application.Users.Commands.ConfirmAccount;

public class ConfirmAccountCommand:IRequest<ConfirmAccountResult>
{
    public Guid UserId { get; set; }   
}