namespace TodoList.MVC.API.Models;

public class TodoItem
{
    public TodoItem(Guid id, Guid userId, string title, string description = "", DateTime dueDate = default,
        bool isCompleted = false)
    {
        Id = id;
        UserId = userId;
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

    //UserId FK
    public Guid UserId { get; set; }
    internal UserAggregateRoot UserAggregateRoot { get; set; }
}