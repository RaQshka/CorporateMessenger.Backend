using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, ConfirmAccountResult>
{
    private readonly UserManager<User> _userManager;
    public ConfirmAccountCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;   
    }
    public async Task<ConfirmAccountResult> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return new ConfirmAccountResult()
            {
                Success = false, 
                Message = "User does not exist"
            };
        }
        if (user.EmailConfirmed != true)
        {
            return new ConfirmAccountResult()
            {
                Success = false,
                Message = "User does not have email confirmed"
            };
        }

        if (user.RegistrationStatus == "Approved")
        {
            return new ConfirmAccountResult()
            {
                Success = true,
                Message = "User is already approved"
            };
        }
        user.RegistrationStatus = "Approved";
        await _userManager.UpdateAsync(user);
        
        return new ConfirmAccountResult()
        {
            Success = true,
            Message = "User approved"
        };
    }
}