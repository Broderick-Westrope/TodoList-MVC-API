namespace TodoList.Application.TodoItems.Queries.GetTodoItem;

public record GetTodoItemResponse(Guid Id, string Title, string Description, DateTime DueDate, bool IsCompleted);