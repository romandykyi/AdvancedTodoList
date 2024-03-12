using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrioritiesAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TodoItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TodoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TodoItemCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TodoListId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItemCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItemCategories_TodoLists_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_CategoryId",
                table: "TodoItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemCategories_TodoListId",
                table: "TodoItemCategories",
                column: "TodoListId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TodoItemCategories_CategoryId",
                table: "TodoItems",
                column: "CategoryId",
                principalTable: "TodoItemCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TodoItemCategories_CategoryId",
                table: "TodoItems");

            migrationBuilder.DropTable(
                name: "TodoItemCategories");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_CategoryId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TodoItems");
        }
    }
}
