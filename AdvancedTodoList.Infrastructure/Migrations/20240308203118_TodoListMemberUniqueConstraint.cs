using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable IDE0079
#pragma warning disable CA1861
#pragma warning disable IDE0300

namespace AdvancedTodoList.Infrastructure.Migrations;

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

#pragma warning restore CA1861
#pragma warning restore IDE0300
#pragma warning restore IDE0079