using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddInvitationLinks : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<bool>(
			name: "Permissions_ManageInvitationLinks",
			table: "TodoListRoles",
			type: "bit",
			nullable: false,
			defaultValue: false);

		migrationBuilder.CreateTable(
			name: "InvitationLinks",
			columns: table => new
			{
				Id = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				TodoListId = table.Column<string>(type: "nvarchar(450)", nullable: false),
				Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
				ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_InvitationLinks", x => x.Id);
				table.ForeignKey(
					name: "FK_InvitationLinks_TodoLists_TodoListId",
					column: x => x.TodoListId,
					principalTable: "TodoLists",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_InvitationLinks_TodoListId",
			table: "InvitationLinks",
			column: "TodoListId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "InvitationLinks");

		migrationBuilder.DropColumn(
			name: "Permissions_ManageInvitationLinks",
			table: "TodoListRoles");
	}
}
