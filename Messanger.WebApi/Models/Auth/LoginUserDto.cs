using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Application.Users.Commands.LoginUser;
using Messenger.Domain;

namespace Notes.WebApi.Models.Auth;

public class LoginUserDto:IMapWith<LoginUserCommand>
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Password { get; set; }= string.Empty;

    /*public void Mapping(Profile profile)
    {
        profile.CreateMap<LoginUserDto, LoginUserCommand>()
            .ForMember(x => x.Title, opt => opt.MapFrom(x => x.Title))
            .ForMember(x => x.Details, opt => opt.MapFrom(x => x.Details));
    }*/
}