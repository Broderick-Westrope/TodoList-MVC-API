using MediatR;
using TodoList.Application.Responses.User;

namespace TodoList.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<GetUserResponse?>;