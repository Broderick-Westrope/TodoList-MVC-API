namespace TodoList.MVC.API.Responses.TodoItem;

public record GetAllTodoItemsResponse(List<GetTodoItemResponse> TodoItems);