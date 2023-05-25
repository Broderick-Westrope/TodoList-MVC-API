using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Repositories;

public class ProjectRepository: Repository<Project>, IProjectRepository
{
    public ProjectRepository(TodoContext context) : base(context) { }

    public TodoContext TodoContext => Context as TodoContext;
}