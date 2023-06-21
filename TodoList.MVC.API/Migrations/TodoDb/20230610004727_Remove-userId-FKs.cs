using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.MVC.API.Migrations.TodoDb
{
    /// <inheritdoc />
    public partial class RemoveuserIdFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserAggregateRootId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserAggregateRootId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Projects");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserAggregateRootId",
                table: "TodoItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserAggregateRootId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UserAggregateRootId",
                table: "Projects",
                column: "UserAggregateRootId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Users_UserAggregateRootId",
                table: "TodoItems",
                column: "UserAggregateRootId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserAggregateRootId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserAggregateRootId",
                table: "TodoItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserAggregateRootId",
                table: "TodoItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TodoItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserAggregateRootId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UserAggregateRootId",
                table: "Projects",
                column: "UserAggregateRootId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Users_UserAggregateRootId",
                table: "TodoItems",
                column: "UserAggregateRootId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
