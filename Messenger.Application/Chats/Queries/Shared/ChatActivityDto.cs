using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.Chats.Queries.Shared;

public class ChatActivityDto :IMapWith<Message>, IMapWith<Document>
{
    public Guid Id { get; set; }
    public string Type { get; set; } // "Message" или "Document"
    public DateTime Timestamp { get; set; }
    public object Data { get; set; } // MessageDto или DocumentDto
    
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt));

        profile.CreateMap<Document, DocumentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.UploadedAt));
    }
}