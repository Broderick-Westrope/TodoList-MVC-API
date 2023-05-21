using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.MVC.API.Migrations.TodoDb
{
    /// <inheritdoc />
    public partial class AddUserIdtoProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Projects");
        }
    }
}
