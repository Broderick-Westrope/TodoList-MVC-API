using MediatR;

namespace TodoList.Application.Projects.Commands.AddProject;

public record AddProjectCommand(CreateProjectRequest Request) : IRequest<AddProjectResult?>;