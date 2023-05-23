namespace TodoList.MVC.API.Responses.User;

public record CreateUserResponse(Guid Id, string Email, string Password);