#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.MVC.API.Migrations.TodoDb;

/// <inheritdoc />
public partial class RenameFKstoUserId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "FK_Projects_Users_UserAggregateRootId",
            "Projects");

        migrationBuilder.DropForeignKey(
            "FK_TodoItems_Users_UserAggregateRootId",
            "TodoItems");

        migrationBuilder.RenameColumn(
            "UserAggregateRootId",
            "TodoItems",
            "UserId");

        migrationBuilder.RenameIndex(
            "IX_TodoItems_UserAggregateRootId",
            table: "TodoItems",
            newName: "IX_TodoItems_UserId");

        migrationBuilder.RenameColumn(
            "UserAggregateRootId",
            "Projects",
            "UserId");

        migrationBuilder.RenameIndex(
            "IX_Projects_UserAggregateRootId",
            table: "Projects",
            newName: "IX_Projects_UserId");

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

        migrationBuilder.RenameColumn(
            "UserId",
            "TodoItems",
            "UserAggregateRootId");

        migrationBuilder.RenameIndex(
            "IX_TodoItems_UserId",
            table: "TodoItems",
            newName: "IX_TodoItems_UserAggregateRootId");

        migrationBuilder.RenameColumn(
            "UserId",
            "Projects",
            "UserAggregateRootId");

        migrationBuilder.RenameIndex(
            "IX_Projects_UserId",
            table: "Projects",
            newName: "IX_Projects_UserAggregateRootId");

        migrationBuilder.AddForeignKey(
            "FK_Projects_Users_UserAggregateRootId",
            "Projects",
            "UserAggregateRootId",
            "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "FK_TodoItems_Users_UserAggregateRootId",
            "TodoItems",
            "UserAggregateRootId",
            "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}