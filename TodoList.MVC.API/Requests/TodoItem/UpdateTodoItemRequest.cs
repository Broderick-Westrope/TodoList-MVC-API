namespace TodoList.MVC.API.Requests.TodoItem;

public record UpdateTodoItemRequest(Guid UserId, string Title, string Description, DateTime DueDate, bool IsCompleted);