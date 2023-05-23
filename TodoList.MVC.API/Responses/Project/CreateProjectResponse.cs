namespace TodoList.MVC.API.Responses.Project;

public record CreateProjectResponse(Guid Id, string Title, Guid UserId);