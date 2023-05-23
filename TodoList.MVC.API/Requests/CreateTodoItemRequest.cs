using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Requests;

public record CreateTodoItemRequest(Guid UserId, string Title, string Description, DateTime DueDate);