using MediatR;

namespace TodoList.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(Guid ProjectId, UpdateProjectRequest Request) : IRequest<UpdateProjectResult>;