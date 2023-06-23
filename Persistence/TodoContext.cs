using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Persistence.Configurations;

namespace TodoList.Persistence;

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