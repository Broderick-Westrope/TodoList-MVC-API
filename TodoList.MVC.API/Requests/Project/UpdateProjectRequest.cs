namespace TodoList.MVC.API.Requests.Project;

public record UpdateProjectRequest(Guid UserId, string Title);