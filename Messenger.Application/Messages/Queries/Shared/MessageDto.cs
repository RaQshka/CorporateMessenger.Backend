using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Messages.Queries.Shared;

public class MessageDto : IMapWith<Message>
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public SenderInfoDto Sender { get; set; } = new SenderInfoDto();
    public string Content { get; set; } = string.Empty;
    public Guid? ReplyToMessageId { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsDeleted { get; set; }
    public List<MessageReactionsOfTypeDto> Reactions { get; set; } = new List<MessageReactionsOfTypeDto>();
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Message, MessageDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ChatId, o => o.MapFrom(s => s.ChatId))
            .ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
            .ForMember(d => d.ReplyToMessageId, o => o.MapFrom(s => s.ReplyToMessageId))
            .ForMember(d => d.SentAt, o => o.MapFrom(s => s.SentAt))
            .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
            .ForMember(d => d.Sender, o => o.MapFrom(s => new SenderInfoDto 
            { 
                Id = s.SenderId,
                FirstName = s.Sender.FirstName, 
                LastName = s.Sender.LastName 
            }))
            .ForMember(d => d.Reactions, o => o.Ignore());
    }
}

public class MessageReactionsOfTypeDto
{
    public int ReactionCount { get; set; }
    public ReactionType ReactionType { get; set; }
}