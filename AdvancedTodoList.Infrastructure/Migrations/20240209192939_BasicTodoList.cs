using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedTodoList.Infrastructure.Migrations;

/// <inheritdoc />
public partial class BasicTodoList : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>(
			name: "Name",
			table: "TodoLists",
			type: "nvarchar(100)",
			maxLength: 100,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(max)");

		migrationBuilder.AddColumn<string>(
			name: "Description",
			table: "TodoLists",
			type: "nvarchar(max)",
			maxLength: 25000,
			nullable: false,
			defaultValue: "");

		migrationBuilder.AlterColumn<string>(
			name: "Name",
			table: "TodoItems",
			type: "nvarchar(100)",
			maxLength: 100,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(max)");

		migrationBuilder.AddColumn<DateTime>(
			name: "DeadlineDate",
			table: "TodoItems",
			type: "datetime2",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Description",
			table: "TodoItems",
			type: "nvarchar(max)",
			maxLength: 10000,
			nullable: false,
			defaultValue: "");

		migrationBuilder.AddColumn<byte>(
			name: "State",
			table: "TodoItems",
			type: "tinyint",
			nullable: false,
			defaultValue: (byte)0);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "Description",
			table: "TodoLists");

		migrationBuilder.DropColumn(
			name: "DeadlineDate",
			table: "TodoItems");

		migrationBuilder.DropColumn(
			name: "Description",
			table: "TodoItems");

		migrationBuilder.DropColumn(
			name: "State",
			table: "TodoItems");

		migrationBuilder.AlterColumn<string>(
			name: "Name",
			table: "TodoLists",
			type: "nvarchar(max)",
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(100)",
			oldMaxLength: 100);

		migrationBuilder.AlterColumn<string>(
			name: "Name",
			table: "TodoItems",
			type: "nvarchar(max)",
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(100)",
			oldMaxLength: 100);
	}
}
