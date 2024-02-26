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
	/// A flag that determines whether user can change a state 
	/// of to-do list items(active/completed/skipped).
	/// </summary>
	public bool HasSetStatePermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can add to-do list items.
	/// </summary>
	public bool HasAddItemsPermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can edit 
	/// to-do list items of other users and the to-do list itself.
	/// </summary>
	public bool HasEditPermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can delete 
	/// to-do list items of other users.
	/// </summary>
	public bool HasDeleteItemsPermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can add members.
	/// </summary>
	public bool HasAddMembersPermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can remove members.
	/// </summary>
	public bool HasRemoveMembersPermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can assign a role to other member.
	/// </summary>
	public bool HasAssignRolesPermission { get; set; } = false;
	/// <summary>
	/// A flag that determines whether user can edit/delete existing roles and add
	/// new roles.
	/// </summary>
	public bool HasEditRolesPermission { get; set; } = false;

	/// <summary>
	/// Max length of the <see cref="Name" /> property.
	/// </summary>
	public const int NameMaxLength = 100;
}
