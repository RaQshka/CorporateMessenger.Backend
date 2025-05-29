using AutoMapper;
using MediatR;
using Messenger.Application.Users.Queries.Shared;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Queries.GetUserInfo;

public class GetFullUserInfoQueryHandler : IRequestHandler<GetFullUserInfoQuery, UserFullDetailsDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetFullUserInfoQueryHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<UserFullDetailsDto> Handle(GetFullUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
            return null;
        
        var dto = _mapper.Map<UserFullDetailsDto>(user);

        return dto;
    }
}