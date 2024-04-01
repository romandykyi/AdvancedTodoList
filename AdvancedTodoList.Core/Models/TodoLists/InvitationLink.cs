using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.TodoLists;

/// <summary>
/// Represents a to-do list invitation link entity.
/// </summary>
public class InvitationLink : IEntity<int>
{
	/// <summary>
	/// A unique identifier for the to-do list item.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Foreign key of the to-do list where the link is active.
	/// </summary>
	[ForeignKey(nameof(TodoList))]
	public required string TodoListId { get; set; }
	/// <summary>
	/// Navigation property to the to-do list associated with this link.
	/// </summary>
	public TodoList TodoList { get; set; } = null!;

	/// <summary>
	/// A unique string value representing the link.
	/// </summary>
	public required string Value { get; set; }

	/// <summary>
	/// Date after which the link becomes invalid.
	/// </summary>
	public DateTime ValidTo { get; set; }
}
