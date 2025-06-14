﻿using MediatR;
using Messenger.Application.Users.Queries.Shared;

namespace Messenger.Application.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<List<UserDetailsDto>>
{
    public string? SearchText { get; set; }
}