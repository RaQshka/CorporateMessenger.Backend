using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain;

namespace Messenger.Application.Chats.Commands.Shared;

public class ChatDto:IMapWith<Chat>
{
    public Guid Id { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public string ChatType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string AccessPolicy { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Chat, ChatDto>();
    }
}