namespace TodoList.Application.Requests.TodoItem;

public record CreateTodoItemRequest(string Title, string Description, DateTime DueDate, Guid UserId);