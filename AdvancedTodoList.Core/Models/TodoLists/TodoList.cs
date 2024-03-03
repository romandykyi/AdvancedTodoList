using AdvancedTodoList.Core.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.TodoLists;

/// <summary>
/// Represents a to-do list entity.
/// </summary>
public class TodoList : IEntity<string>
{
	/// <summary>
	/// An unique identifier for the to-do list.
	/// </summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public string Id { get; set; } = null!;

	/// <summary>
	/// Name (title) of the to-do list.
	/// </summary>
	[MaxLength(NameMaxLength)]
	public required string Name { get; set; } = null!;
	/// <summary>
	/// Description of the to-do list.
	/// </summary>
	[MaxLength(DescriptionMaxLength)]
	public required string Description { get; set; } = null!;

	/// <summary>
	/// Foreign key referencing the user who created this to-do list.
	/// </summary>
	[ForeignKey(nameof(Owner))]
	public required string? OwnerId { get; set; } = null!;
	/// <summary>
	/// Navigation property to the user who created this to-do list.
	/// </summary>
	public ApplicationUser? Owner { get; set; }

	/// <summary>
	/// Maximum allowed length of <see cref="Name"/>.
	/// </summary>
	public const int NameMaxLength = 100;
	/// <summary>
	/// Maximum allowed length of <see cref="Description"/>.
	/// </summary>
	public const int DescriptionMaxLength = 25_000;

	/// <summary>
	/// Collection of to-do items associated with this todo list.
	/// </summary>
	public virtual ICollection<TodoItem> TodoItems { get; set; } = null!;
}
