#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.MVC.API.Migrations.TodoDb;

/// <inheritdoc />
public partial class AddProjectTodoItemMapping : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "ProjectTodoItemMappings",
            table => new
            {
                ProjectId = table.Column<Guid>("uniqueidentifier", nullable: false),
                TodoItemId = table.Column<Guid>("uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProjectTodoItemMappings", x => new { x.ProjectId, x.TodoItemId });
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "ProjectTodoItemMappings");
    }
}