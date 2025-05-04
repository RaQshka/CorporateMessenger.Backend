using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain;
using Messenger.Domain.Entities;

namespace Messenger.Application.Users.Queries.Shared;

public class UserDetailsDto:IMapWith<User>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; }= string.Empty;
    public string RegistrationStatus { get; set; }= string.Empty;
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLogin  { get; set; }
    public IList<string> Roles { get; set; }= new List<string>();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserDetailsDto>()
            .ForMember(d => d.Id, 
                o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Username, 
                o => o.MapFrom(s => s.UserName))
            .ForMember(d=>d.FirstName, 
                o => o.MapFrom(s => s.FirstName))
            .ForMember(d=>d.LastName, 
                o => o.MapFrom(s => s.LastName))
            .ForMember(d => d.Email, 
                o => o.MapFrom(s => s.Email))
            .ForMember(d => d.RegistrationStatus, 
                o => o.MapFrom(s => s.RegistrationStatus))
            .ForMember(d => d.LastLogin, 
                o => o.MapFrom(s => s.LastLogin))
            .ForMember(d => d.EmailConfirmed, 
                o => o.MapFrom(s => s.EmailConfirmed))
            .ForMember(dest => 
                dest.Roles, opt => opt.Ignore()); // Добавим вручную в Handler

    }
}