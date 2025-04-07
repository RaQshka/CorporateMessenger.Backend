using MediatR;
using Messenger.Application.Chats.Commands.Shared;

namespace Messenger.Application.Chats.Commands.CreateChat;

public class CreateChatCommand : IRequest<ChatDto>
{
    public string ChatName { get; set; } = string.Empty;
    public string ChatType { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public string AccessPolicy { get; set; } = string.Empty;
}