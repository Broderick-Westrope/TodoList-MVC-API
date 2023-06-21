namespace TodoList.MVC.API.DbModels;

public class Project
{
    public Project(Guid id, string title)
    {
        Id = id;
        Title = title;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
}