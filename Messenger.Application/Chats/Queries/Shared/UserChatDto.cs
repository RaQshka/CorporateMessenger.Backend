using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;

namespace Messenger.Application.Chats.Queries.Shared;

public class UserChatDto:IMapWith<Chat>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Chat, UserChatDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest =>dest.Name, opt=>opt.MapFrom(src=>src.ChatName))
            .ForMember(dest =>dest.Type, opt=>opt.MapFrom(src=>src.ChatType));
    }
}