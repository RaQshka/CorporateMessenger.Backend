﻿using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain;
using Messenger.Domain.Entities;

namespace Messenger.Application.Users.Queries.Shared;

public class UserDetailsDto:IMapWith<User>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; }= string.Empty;
    public DateTime? LastLogin  { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserDetailsDto>();

    }
}