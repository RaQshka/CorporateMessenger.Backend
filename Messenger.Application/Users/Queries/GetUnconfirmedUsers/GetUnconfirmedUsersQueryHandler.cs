﻿using AutoMapper;
using MediatR;
using Messenger.Application.Users.Queries.Shared;
using Microsoft.AspNetCore.Identity;
using Messenger.Domain;
using Messenger.Domain.Entities;

namespace Messenger.Application.Users.Queries.GetUnconfirmedUsers;

public class GetUnconfirmedUsersQueryHandler : IRequestHandler<GetUnconfirmedUsersQuery, List<UserFullDetailsDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetUnconfirmedUsersQueryHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<UserFullDetailsDto>> Handle(GetUnconfirmedUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users
            .Where(u => !u.EmailConfirmed || u.RegistrationStatus != "Approved")
            .ToList();
        
        var dtos = new List<UserFullDetailsDto>();
        foreach (var user in users)
        {
            var dto = _mapper.Map<UserFullDetailsDto>(user);
            dto.Roles = await _userManager.GetRolesAsync(user);
            dtos.Add(dto);
        }
        
        return dtos;
    }
}
