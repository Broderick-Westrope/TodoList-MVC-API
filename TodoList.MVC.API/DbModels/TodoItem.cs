namespace TodoList.MVC.API.Models;

public class TodoItem
{
    public TodoItem(Guid id, string title, string description = "", DateTime dueDate = default,
        bool isCompleted = false)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        IsCompleted = isCompleted;
    }
    
    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
}