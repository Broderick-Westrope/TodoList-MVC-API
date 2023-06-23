using MediatR;

namespace TodoList.Application.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid ProjectId) : IRequest<DeleteProjectResult>;