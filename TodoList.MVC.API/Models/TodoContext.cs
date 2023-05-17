using Microsoft.EntityFrameworkCore;

namespace TodoList.MVC.API.Models;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

    public DbSet<Task> Tasks { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;
}