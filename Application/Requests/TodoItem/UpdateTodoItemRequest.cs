namespace TodoList.Application.Requests.TodoItem;

//? Should this include the userId? What's the use case for changing this?
public record UpdateTodoItemRequest(string Title, string Description, DateTime DueDate, bool IsCompleted);