using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.TodoLists.Members;

/// <summary>
/// A model which represents a role inside the to-do list with its own permissions.
/// </summary>
public class TodoListRole : IEntity<int>, ITodoListDependant
{
	/// <summary>
	/// A unique identifier.
	/// </summary>
	[Key]
	public int Id { get; set; }
	/// <summary>
	/// Name of the role.
	/// </summary>
	[MaxLength(NameMaxLength)]
	public required string Name { get; set; }

	/// <summary>
	/// A foreign key of the to-do list which has this role.
	/// </summary>
	[ForeignKey(nameof(TodoList))]
	public required string TodoListId { get; set; }
	/// <summary>
	/// A navigation property to the to-do list which has this role.
	/// </summary>
	public TodoList TodoList { get; set; } = null!;

	/// <summary>
	/// Permissions which each member with this role has.
	/// </summary>
	public RolePermissions Permissions { get; set; } = new();

	/// <summary>
	/// Max length of the <see cref="Name" /> property.
	/// </summary>
	public const int NameMaxLength = 100;
}
