namespace TodoList.MVC.API.Requests;

public record CreateProjectRequest(Guid UserId, string Title);