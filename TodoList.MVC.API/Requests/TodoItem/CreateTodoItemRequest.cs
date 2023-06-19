namespace TodoList.MVC.API.Requests.TodoItem;

public record CreateTodoItemRequest(string Title, string Description, DateTime DueDate, Guid UserId);