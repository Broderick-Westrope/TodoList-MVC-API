using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.MVC.API.Migrations.TodoDb
{
    /// <inheritdoc />
    public partial class RenameFKstoUserId : Migration
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

            migrationBuilder.RenameColumn(
                name: "UserAggregateRootId",
                table: "TodoItems",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_UserAggregateRootId",
                table: "TodoItems",
                newName: "IX_TodoItems_UserId");

            migrationBuilder.RenameColumn(
                name: "UserAggregateRootId",
                table: "Projects",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_UserAggregateRootId",
                table: "Projects",
                newName: "IX_Projects_UserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserId",
                table: "TodoItems");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TodoItems",
                newName: "UserAggregateRootId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_UserId",
                table: "TodoItems",
                newName: "IX_TodoItems_UserAggregateRootId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Projects",
                newName: "UserAggregateRootId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                newName: "IX_Projects_UserAggregateRootId");

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
    }
}
