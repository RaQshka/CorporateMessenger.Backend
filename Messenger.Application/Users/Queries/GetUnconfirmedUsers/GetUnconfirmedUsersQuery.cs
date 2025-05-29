using MediatR;
using Messenger.Application.Users.Queries.Shared;

namespace Messenger.Application.Users.Queries.GetUnconfirmedUsers;

public class GetUnconfirmedUsersQuery:IRequest<List<UserFullDetailsDto>>
{
    
}