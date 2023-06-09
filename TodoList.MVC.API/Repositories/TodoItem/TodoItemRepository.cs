using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Repositories;

public class TodoItemRepository : Repository<TodoItem>, ITodoItemRepository
{
    public TodoItemRepository(TodoContext context) : base(context)
    {
    }

    public TodoContext TodoContext => Context as TodoContext;
}