#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.MVC.API.Migrations.TodoDb;

/// <inheritdoc />
public partial class MovetoUserAggregateRoot : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
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

        migrationBuilder.AddColumn<Guid>(
            "UserAggregateRootId",
            "TodoItems",
            "uniqueidentifier",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            "UserAggregateRootId",
            "Projects",
            "uniqueidentifier",
            nullable: true);

        migrationBuilder.CreateIndex(
            "IX_TodoItems_UserAggregateRootId",
            "TodoItems",
            "UserAggregateRootId");

        migrationBuilder.CreateIndex(
            "IX_Projects_UserAggregateRootId",
            "Projects",
            "UserAggregateRootId");

        migrationBuilder.AddForeignKey(
            "FK_Projects_Users_UserAggregateRootId",
            "Projects",
            "UserAggregateRootId",
            "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            "FK_TodoItems_Users_UserAggregateRootId",
            "TodoItems",
            "UserAggregateRootId",
            "Users",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "FK_Projects_Users_UserAggregateRootId",
            "Projects");

        migrationBuilder.DropForeignKey(
            "FK_TodoItems_Users_UserAggregateRootId",
            "TodoItems");

        migrationBuilder.DropIndex(
            "IX_TodoItems_UserAggregateRootId",
            "TodoItems");

        migrationBuilder.DropIndex(
            "IX_Projects_UserAggregateRootId",
            "Projects");

        migrationBuilder.DropColumn(
            "UserAggregateRootId",
            "TodoItems");

        migrationBuilder.DropColumn(
            "UserAggregateRootId",
            "Projects");

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
}