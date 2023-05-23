namespace TodoList.MVC.API.Responses.TodoItem;

public record GetTodoItemResponse(Guid Id, string Title, string Description, DateTime DueDate, bool IsCompleted,
    Guid UserId);