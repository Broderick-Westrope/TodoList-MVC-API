using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.MVC.API.Migrations.TodoDb
{
    /// <inheritdoc />
    public partial class MovetoUserAggregateRoot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_UserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAggregateRootId",
                table: "TodoItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserAggregateRootId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_UserAggregateRootId",
                table: "TodoItems",
                column: "UserAggregateRootId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserAggregateRootId",
                table: "Projects",
                column: "UserAggregateRootId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserAggregateRootId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserAggregateRootId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_UserAggregateRootId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserAggregateRootId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserAggregateRootId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "UserAggregateRootId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_UserId",
                table: "TodoItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Users_UserId",
                table: "TodoItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
