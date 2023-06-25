namespace TodoList.Application.Projects.Commands.AddProject;

public record CreateProjectRequest(string Title, Guid UserId);