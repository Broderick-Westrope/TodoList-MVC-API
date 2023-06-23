namespace TodoList.Application.Requests.Project;

public record CreateProjectRequest(string Title, Guid UserId);