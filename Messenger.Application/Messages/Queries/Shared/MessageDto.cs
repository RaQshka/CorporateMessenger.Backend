using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;

namespace Messenger.Application.Messages.Queries.Shared;

public class MessageDto : IMapWith<Message>
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ReplyToMessageId { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsDeleted { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Message, MessageDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ChatId, o => o.MapFrom(s => s.ChatId))
            .ForMember(d => d.SenderId, o => o.MapFrom(s => s.SenderId))
            .ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
            .ForMember(d => d.ReplyToMessageId, o => o.MapFrom(s => s.ReplyToMessageId))
            .ForMember(d => d.SentAt, o => o.MapFrom(s => s.SentAt))
            .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted));
    }
}