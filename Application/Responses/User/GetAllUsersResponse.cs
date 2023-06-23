namespace TodoList.Application.Responses.User;

public record GetAllUsersResponse(List<GetUserResponse> Users);