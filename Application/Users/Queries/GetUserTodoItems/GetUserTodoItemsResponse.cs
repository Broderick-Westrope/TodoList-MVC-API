using TodoList.Application.TodoItems.Queries.GetTodoItem;

namespace TodoList.Application.Users.Queries.GetUserTodoItems;

public record GetUserTodoItemsResponse(List<GetTodoItemResponse> TodoItems);