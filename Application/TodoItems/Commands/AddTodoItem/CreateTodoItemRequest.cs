namespace TodoList.Application.TodoItems.Commands.AddTodoItem;

public record CreateTodoItemRequest(string Title, string Description, DateTime DueDate, Guid UserId);