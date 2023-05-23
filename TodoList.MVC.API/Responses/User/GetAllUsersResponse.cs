namespace TodoList.MVC.API.Responses.User;

public record GetAllUsersResponse(List<GetUserResponse> Users);