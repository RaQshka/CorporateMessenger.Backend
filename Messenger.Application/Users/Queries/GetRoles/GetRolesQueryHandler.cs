using AutoMapper;
using MediatR;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Queries.GetRoles;

public class GetRolesQueryHandler: IRequestHandler<GetRolesQuery, IEnumerable<RolesDto>>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    public GetRolesQueryHandler(RoleManager<Role> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<RolesDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = _roleManager.Roles.ToList();
        return _mapper.Map<List<RolesDto>>(roles);
    }
}