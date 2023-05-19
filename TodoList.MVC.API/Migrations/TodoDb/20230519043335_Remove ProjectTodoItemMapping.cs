using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.MVC.API.Migrations.TodoDb
{
    /// <inheritdoc />
    public partial class RemoveProjectTodoItemMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTodoItemMappings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTodoItemMappings",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TodoItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTodoItemMappings", x => new { x.ProjectId, x.TodoItemId });
                });
        }
    }
}
