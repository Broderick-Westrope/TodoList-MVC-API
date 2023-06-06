#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.MVC.API.Migrations.TodoDb;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Projects",
            table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                Title = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Projects", x => x.Id); });

        migrationBuilder.CreateTable(
            "TodoItems",
            table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>("uniqueidentifier", nullable: false),
                Title = table.Column<string>("nvarchar(max)", nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: false),
                DueDate = table.Column<DateTime>("datetime2", nullable: false),
                IsCompleted = table.Column<bool>("bit", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_TodoItems", x => x.Id); });

        migrationBuilder.CreateTable(
            "Users",
            table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                Email = table.Column<string>("nvarchar(max)", nullable: false),
                Password = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Projects");

        migrationBuilder.DropTable(
            "TodoItems");

        migrationBuilder.DropTable(
            "Users");
    }
}