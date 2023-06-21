namespace TodoList.MVC.API.Responses.TodoItem;

public record GetUserTodoItemsResponse(List<GetTodoItemResponse> TodoItems);