using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Messages.Queries.Shared;

public class MessageReactionDto : IMapWith<MessageReaction>
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<MessageReaction, MessageReactionDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.MessageId, o => o.MapFrom(s => s.MessageId))
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.ReactionType, o => o.MapFrom(s => s.ReactionType))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt));
    }
}