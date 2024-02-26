using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMembersAndRolesEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoListRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TodoListId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HasSetStatePermission = table.Column<bool>(type: "bit", nullable: false),
                    HasAddItemsPermission = table.Column<bool>(type: "bit", nullable: false),
                    HasEditPermission = table.Column<bool>(type: "bit", nullable: false),
                    HasDeleteItemsPermission = table.Column<bool>(type: "bit", nullable: false),
                    HasAddMembersPermission = table.Column<bool>(type: "bit", nullable: false),
                    HasRemoveMembersPermission = table.Column<bool>(type: "bit", nullable: false),
                    HasAssignRolesPermission = table.Column<bool>(type: "bit", nullable: false),
                    HasEditRolesPermission = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoListRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoListRoles_TodoLists_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TodoListsMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TodoListId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoListsMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoListsMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoListsMembers_TodoListRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TodoListRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TodoListsMembers_TodoLists_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoListRoles_TodoListId",
                table: "TodoListRoles",
                column: "TodoListId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListsMembers_RoleId",
                table: "TodoListsMembers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListsMembers_TodoListId",
                table: "TodoListsMembers",
                column: "TodoListId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListsMembers_UserId",
                table: "TodoListsMembers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoListsMembers");

            migrationBuilder.DropTable(
                name: "TodoListRoles");
        }
    }
}
