#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.MVC.API.Migrations.TodoDb;

/// <inheritdoc />
public partial class AddFlientAPIFKs : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            "IX_TodoItems_UserId",
            "TodoItems",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Projects_UserId",
            "Projects",
            "UserId");

        migrationBuilder.AddForeignKey(
            "FK_Projects_Users_UserId",
            "Projects",
            "UserId",
            "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "FK_TodoItems_Users_UserId",
            "TodoItems",
            "UserId",
            "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "FK_Projects_Users_UserId",
            "Projects");

        migrationBuilder.DropForeignKey(
            "FK_TodoItems_Users_UserId",
            "TodoItems");

        migrationBuilder.DropIndex(
            "IX_TodoItems_UserId",
            "TodoItems");

        migrationBuilder.DropIndex(
            "IX_Projects_UserId",
            "Projects");
    }
}