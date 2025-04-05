using MediatR;
using Messenger.Application.Users.Queries.Shared;

namespace Messenger.Application.Users.Queries.GetUserInfo;

public class GetUserInfoQuery : IRequest<UserDetailsDto>
{
    public Guid UserId { get; set; }
}