using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListsAndItemsOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "TodoLists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "TodoItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_OwnerId",
                table: "TodoLists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_OwnerId",
                table: "TodoItems",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_OwnerId",
                table: "TodoItems",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_AspNetUsers_OwnerId",
                table: "TodoLists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_OwnerId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_AspNetUsers_OwnerId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoLists_OwnerId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_OwnerId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TodoItems");
        }
    }
}
