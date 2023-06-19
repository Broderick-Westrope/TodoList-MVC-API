using System.Collections.ObjectModel;

namespace TodoList.MVC.API.Models;

public class UserAggregate
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    private readonly List<TodoItem> _todoItems = new List<TodoItem>();
    private readonly List<Project> _projects = new();

    public IReadOnlyCollection<TodoItem>? TodoItems => _todoItems.AsReadOnly();
    public IReadOnlyCollection<Project>? Projects => _projects.AsReadOnly();
    
    public UserAggregate(Guid id, string email, string password)
    {
        Id = id;
        Email = email;
        Password = password;
    }

    public void AddTodoItem(TodoItem todoItem)
    {
        _todoItems.Add(todoItem);
    }
    
    public void AddTodoItems(IEnumerable<TodoItem> todoItem)
    {
        _todoItems.AddRange(todoItem);
    }

    public void DeleteTodoItem(Guid todoItemId)
    {
        var todoItem = _todoItems.FirstOrDefault(x => x.Id == todoItemId);
        if (todoItem != null) _todoItems.Remove(todoItem);
    }
    
    public void AddProject(Project project)
    {
        _projects.Add(project);
    }

    public void AddProjects(IEnumerable<Project> projects)
    {
        _projects.AddRange(projects);
    }
    
    public void DeleteProjectById(Guid projectId)
    {
        var project = _projects.FirstOrDefault(x => x.Id == projectId);
        if (project != null) _projects.Remove(project);
    }
}