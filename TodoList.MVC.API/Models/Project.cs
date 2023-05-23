namespace TodoList.MVC.API.Models;

public class Project
{
    public Project(Guid id, Guid userId, string title)
    {
        Id = id;
        UserId = userId;
        Title = title;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }

    // UserId FK
    public Guid UserId { get; set; }
    public User User { get; set; }
}