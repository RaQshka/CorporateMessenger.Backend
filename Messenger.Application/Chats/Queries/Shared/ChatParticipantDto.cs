using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;

namespace Messenger.Application.Chats.Queries.Shared;

public class ChatParticipantDto: IMapWith<ChatParticipant>
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ChatParticipant, ChatParticipantDto>();
    }
    
}