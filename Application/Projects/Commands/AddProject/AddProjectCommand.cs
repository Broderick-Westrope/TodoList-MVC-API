using MediatR;
using TodoList.Application.Requests.Project;

namespace TodoList.Application.Projects.Commands.AddProject;

public record AddProjectCommand(CreateProjectRequest Request) : IRequest<AddProjectResult?>;