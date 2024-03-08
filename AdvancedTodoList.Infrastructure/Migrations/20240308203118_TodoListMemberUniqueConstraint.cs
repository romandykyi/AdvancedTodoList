using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TodoListMemberUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoListsMembers_UserId",
                table: "TodoListsMembers");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListsMembers_UserId_TodoListId",
                table: "TodoListsMembers",
                columns: new[] { "UserId", "TodoListId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoListsMembers_UserId_TodoListId",
                table: "TodoListsMembers");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListsMembers_UserId",
                table: "TodoListsMembers",
                column: "UserId");
        }
    }
}
