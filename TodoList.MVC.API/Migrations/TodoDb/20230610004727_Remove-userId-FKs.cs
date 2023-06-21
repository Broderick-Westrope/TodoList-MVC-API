#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoList.MVC.API.Migrations.TodoDb;

/// <inheritdoc />
public partial class RemoveuserIdFKs : Migration
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

        migrationBuilder.DropColumn(
            "UserId",
            "TodoItems");

        migrationBuilder.DropColumn(
            "UserId",
            "Projects");

        migrationBuilder.AlterColumn<Guid>(
            "UserAggregateRootId",
            "TodoItems",
            "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldNullable: true);

        migrationBuilder.AlterColumn<Guid>(
            "UserAggregateRootId",
            "Projects",
            "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldNullable: true);

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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "FK_Projects_Users_UserAggregateRootId",
            "Projects");

        migrationBuilder.DropForeignKey(
            "FK_TodoItems_Users_UserAggregateRootId",
            "TodoItems");

        migrationBuilder.AlterColumn<Guid>(
            "UserAggregateRootId",
            "TodoItems",
            "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AddColumn<Guid>(
            "UserId",
            "TodoItems",
            "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AlterColumn<Guid>(
            "UserAggregateRootId",
            "Projects",
            "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AddColumn<Guid>(
            "UserId",
            "Projects",
            "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
}