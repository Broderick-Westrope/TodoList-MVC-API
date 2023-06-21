using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.DbModels;
using TodoList.MVC.API.DbModels.Configuration;

namespace TodoList.MVC.API;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;

    public DbSet<UserAggregate> Users { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}