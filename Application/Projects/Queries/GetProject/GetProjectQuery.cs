using MediatR;

namespace TodoList.Application.Projects.Queries.GetProject;

public record GetProjectQuery(Guid ProjectId) : IRequest<GetProjectResponse?>;