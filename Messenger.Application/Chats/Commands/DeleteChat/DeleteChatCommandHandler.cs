using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.DeleteChat;

public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, bool>
{
    private readonly IChatRepository _chatRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    public DeleteChatCommandHandler(IChatRepository chatRepository, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _chatRepository = chatRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return false;
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            
        }
        
        
        await _chatRepository.DeleteChatAsync(request.ChatId, cancellationToken);
        return true;
    }
    
    
}