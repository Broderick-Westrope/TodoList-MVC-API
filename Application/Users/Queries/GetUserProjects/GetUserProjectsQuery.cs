using MediatR;
using TodoList.Application.Responses.User;

namespace TodoList.Application.Users.Queries.GetUserProjects;

public record GetUserProjectsQuery(Guid UserId) : IRequest<GetUserProjectsResponse?>;