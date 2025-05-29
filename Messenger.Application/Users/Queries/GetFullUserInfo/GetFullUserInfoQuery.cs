using MediatR;
using Messenger.Application.Users.Queries.Shared;

namespace Messenger.Application.Users.Queries.GetUserInfo;

public class GetFullUserInfoQuery : IRequest<UserFullDetailsDto>
{
    public Guid UserId { get; set; }
}