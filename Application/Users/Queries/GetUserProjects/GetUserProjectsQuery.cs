using MediatR;

namespace TodoList.Application.Users.Queries.GetUserProjects;

public record GetUserProjectsQuery(Guid UserId) : IRequest<GetUserProjectsResponse?>;