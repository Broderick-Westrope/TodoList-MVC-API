#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.Persistence.Migrations.TodoDb;

/// <inheritdoc />
public partial class RemoveProjectTodoItemMapping : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "ProjectTodoItemMappings");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
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
}