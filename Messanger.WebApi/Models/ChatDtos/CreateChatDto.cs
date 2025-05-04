using Messenger.Domain.Enums;

namespace Messenger.WebApi.Models.ChatDtos;

public class CreateChatDto
{
    public string Name { get; set; }
    public ChatTypes Type { get; set; }
}