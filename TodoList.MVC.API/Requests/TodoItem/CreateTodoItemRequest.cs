namespace TodoList.MVC.API.Requests.TodoItem;

public record CreateTodoItemRequest(Guid UserId, string Title, string Description, DateTime DueDate);