using System.ComponentModel.DataAnnotations;

namespace TodoList.MVC.API.Models;

public class Project
{
    public Project(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
    }

    [Key] public Guid Id { get; set; }

    public string Title { get; set; }
}