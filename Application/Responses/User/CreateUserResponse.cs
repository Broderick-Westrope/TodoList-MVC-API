namespace TodoList.Application.Responses.User;

public record CreateUserResponse(Guid Id, string Email, string Password);