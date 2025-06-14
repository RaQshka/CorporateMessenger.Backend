﻿using AutoMapper;
using MediatR;
using Messenger.Application.Users.Queries.Shared;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDetailsDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<UserDetailsDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();
        
        var dtos = new List<UserDetailsDto>();

        foreach (var user in users)
        {
            var dto = _mapper.Map<UserDetailsDto>(user);
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