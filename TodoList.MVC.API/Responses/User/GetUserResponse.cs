namespace TodoList.MVC.API.Responses.User;

public record GetUserResponse(Guid Id, string Email, string Password, List<Guid> TodoItemIds, List<Guid> ProjectIds);