using System.ComponentModel.DataAnnotations;

namespace TodoList.MVC.API.Models;

public class TodoItem
{
    public TodoItem(Guid userId, string title, string description = "", DateTime dueDate = default)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        IsCompleted = false;
    }

    [Key] public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
}