using MediatR;

namespace Messenger.Application.Users.Queries.GetRoles;

public class GetRolesQuery: IRequest<IEnumerable<RolesDto>>
{
    
}