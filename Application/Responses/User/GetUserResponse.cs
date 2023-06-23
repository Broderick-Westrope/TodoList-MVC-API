namespace TodoList.Application.Responses.User;

public record GetUserResponse(Guid Id, string Email, string Password);