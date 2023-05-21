using System.ComponentModel.DataAnnotations;

namespace TodoList.MVC.API.Models;

public class Project
{
    public Project(Guid userId, string title)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
    }

    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
}