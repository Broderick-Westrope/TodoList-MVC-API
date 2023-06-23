namespace TodoList.Application.Responses.TodoItem;

public record GetUserTodoItemsResponse(List<GetTodoItemResponse> TodoItems);