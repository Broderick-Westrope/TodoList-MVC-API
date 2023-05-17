namespace TodoList.MVC.API.Models;

public class Task
{
    public Task(string title)
    {
        Title = title;
        IsCompleted = false;
    }

    public long TaskId { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}