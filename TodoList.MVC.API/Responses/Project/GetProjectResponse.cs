namespace TodoList.MVC.API.Responses.Project;

public record GetProjectResponse(Guid Id, string Title, Guid UserId);