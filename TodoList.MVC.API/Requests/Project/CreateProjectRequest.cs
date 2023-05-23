namespace TodoList.MVC.API.Requests.Project;

public record CreateProjectRequest(Guid UserId, string Title);