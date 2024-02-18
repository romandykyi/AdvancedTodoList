using AdvancedTodoList.Core.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.TodoLists.Members;

/// <summary>
/// A model which represents a member of the to-do list and his/her role.
/// </summary>
public class TodoListMember : IEntity<int>
{
	/// <summary>
	/// An unique identifier.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// A foreign key of the user who is the member.
	/// </summary>
	[ForeignKey(nameof(User))]
	public required string UserId { get; set; }
	/// <summary>
	/// A foreign key of the to-do list. 
	/// </summary>
	[ForeignKey(nameof(TodoList))]
	public required string TodoListId { get; set; }
	/// <summary>
	/// A foreign key of the role, if null user have no role
	/// and has a read-only access.
	/// </summary>
	[ForeignKey(nameof(Role))]
	public int? RoleId { get; set; }

	/// <summary>
	/// A navigation property to the user who is the member.
	/// </summary>
	public ApplicationUser User { get; set; } = null!;
	/// <summary>
	/// A navigation property to the to-do list.
	/// </summary>
	public TodoList TodoList { get; set; } = null!;
	/// <summary>
	/// A navigation property to the role.
	/// </summary>
	public TodoListMemberRole Role { get; set; } = null!;
}
