using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Requests;

public record CreateProjectRequest(Guid UserId, string Title);