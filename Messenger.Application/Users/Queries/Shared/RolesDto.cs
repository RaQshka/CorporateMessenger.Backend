using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain.Entities;

namespace Messenger.Application.Users.Queries.GetRoles;

public class RolesDto:IMapWith<Role>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Role, RolesDto>();
    }
}