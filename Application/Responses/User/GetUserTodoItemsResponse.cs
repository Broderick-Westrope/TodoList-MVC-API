using TodoList.Application.Responses.TodoItem;

namespace TodoList.Application.Responses.User;

public record GetUserTodoItemsResponse(List<GetTodoItemResponse> TodoItems);