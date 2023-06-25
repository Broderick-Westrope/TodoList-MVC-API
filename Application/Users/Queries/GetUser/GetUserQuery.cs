using MediatR;

namespace TodoList.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<GetUserResponse?>;