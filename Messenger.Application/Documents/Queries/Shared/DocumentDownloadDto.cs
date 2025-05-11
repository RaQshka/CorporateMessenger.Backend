using AutoMapper;

namespace Messenger.Application.Documents.Queries.Shared;

public class DocumentDownloadDto
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<(byte[] Content, string FileName, string ContentType), DocumentDownloadDto>()
            .ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
            .ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
            .ForMember(d => d.ContentType, o => o.MapFrom(s => s.ContentType));
    }
}