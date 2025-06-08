using AutoMapper;
using MediatR;
using Messenger.Application.Users.Queries.Shared;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Queries.GetUsers;

public class GetFullUsersQueryHandler : IRequestHandler<GetFullUsersQuery, List<UserFullDetailsDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetFullUsersQueryHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<UserFullDetailsDto>> Handle(GetFullUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();
        
        var dtos = new List<UserFullDetailsDto>();

        foreach (var user in users)
        {
            var dto = _mapper.Map<UserFullDetailsDto>(user);
            dto.Roles = await _userManager.GetRolesAsync(user);
            dtos.Add(dto);
        }

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            dtos= dtos.Where(x=>x.FirstName.Contains(request.SearchText) || 
                                x.LastName.Contains(request.SearchText)).ToList();
        }
        
        return dtos;
    }
}