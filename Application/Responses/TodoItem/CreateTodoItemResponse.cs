namespace TodoList.Application.Responses.TodoItem;

public record CreateTodoItemResponse(Guid Id, string Title, string Description, DateTime DueDate,
    bool IsCompleted = false);