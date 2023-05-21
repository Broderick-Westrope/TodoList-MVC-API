using Microsoft.EntityFrameworkCore;

namespace TodoList.MVC.API.Models;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;
}