namespace TodoList.Application.Responses.TodoItem;

public record GetTodoItemResponse(Guid Id, string Title, string Description, DateTime DueDate, bool IsCompleted);