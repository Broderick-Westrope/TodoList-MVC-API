namespace TodoList.Application.Users.Queries.GetUser;

public record GetUserResponse(Guid Id, string Email, string Password);