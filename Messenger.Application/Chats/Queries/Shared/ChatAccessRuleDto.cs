using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;

namespace Messenger.Application.Chats.Queries.Shared;

public class ChatAccessRuleDto:IMapWith<ChatAccessRule>
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int AccessMask { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ChatAccessRule, ChatAccessRuleDto>()
            .ForMember(dest => dest.RoleName, 
                opt => opt.MapFrom(src => src.Role.Name));
    }
}