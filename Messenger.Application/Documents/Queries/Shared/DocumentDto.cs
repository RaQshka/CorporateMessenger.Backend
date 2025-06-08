using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.Documents.Queries.Shared;

public class DocumentDto : IMapWith<Document>
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid UploaderId { get; set; }
    public SenderInfoDto Sender { get; set; } = new SenderInfoDto(); 
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Document, DocumentDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ChatId, o => o.MapFrom(s => s.ChatId))
            .ForMember(d => d.UploaderId, o => o.MapFrom(s => s.UploaderId))
            .ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
            .ForMember(d => d.FileType, o => o.MapFrom(s => s.FileType))
            .ForMember(d => d.FileSize, o => o.MapFrom(s => s.FileSize))
            .ForMember(d => d.UploadedAt, o => o.MapFrom(s => s.UploadedAt))
            .ForMember(d => d.Sender, o => o.Ignore());
    }
}