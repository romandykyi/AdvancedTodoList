using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRolePriorityColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Priority",
            table: "TodoListRoles",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Priority",
            table: "TodoListRoles");
    }
}
