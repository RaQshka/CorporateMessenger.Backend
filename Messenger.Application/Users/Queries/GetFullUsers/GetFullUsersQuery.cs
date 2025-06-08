using MediatR;
using Messenger.Application.Users.Queries.Shared;

namespace Messenger.Application.Users.Queries.GetUsers;

public class GetFullUsersQuery : IRequest<List<UserFullDetailsDto>>
{
    public string? SearchText { get; set; }
}