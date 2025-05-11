using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;

namespace Messenger.Application.Documents.Queries.Shared;

public class DocumentAccessRuleDto : IMapWith<DocumentAccessRule>
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid RoleId { get; set; }
    public int DocumentAccessMask { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DocumentAccessRule, DocumentAccessRuleDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.DocumentId, o => o.MapFrom(s => s.DocumentId))
            .ForMember(d => d.RoleId, o => o.MapFrom(s => s.RoleId))
            .ForMember(d => d.DocumentAccessMask, o => o.MapFrom(s => s.DocumentAccessMask));
    }
}