namespace TodoList.MVC.API.Models;

public class UserAggregate
{
    public UserAggregate(Guid id, string email, string password)
    {
        Id = id;
        Email = email;
        Password = password;
        TodoItems = new List<TodoItem>();
        Projects = new List<Project>();
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ICollection<TodoItem> TodoItems { get; set; }
    public ICollection<Project> Projects { get; set; }
}