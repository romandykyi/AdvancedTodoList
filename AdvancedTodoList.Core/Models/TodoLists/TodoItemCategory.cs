using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.TodoLists;

/// <summary>
/// Represents a to-do list items category entity.
/// </summary>
public class TodoItemCategory : IEntity<int>, ITodoListDependant, IHasName
{
	/// <summary>
	/// An unique identifier for the category.
	/// </summary>
	[Key]
	public int Id { get; set; }
	/// <summary>
	/// Name of the category.
	/// </summary>
	[MaxLength(NameMaxLength)]
	public required string Name { get; set; }

	/// <summary>
	/// Foreign key referencing the associated to-do list.
	/// </summary>
	[ForeignKey(nameof(TodoList))]
	public required string TodoListId { get; set; }
	/// <summary>
	/// Navigation property to the to-do list associated with this to-do item.
	/// </summary>
	public TodoList TodoList { get; set; } = null!;

	/// <summary>
	/// Collection of to-do items associated with this category.
	/// </summary>
	public virtual ICollection<TodoItem> TodoItems { get; set; } = null!;

	/// <summary>
	/// Maximum allowed length of <see cref="Name"/>.
	/// </summary>
	public const int NameMaxLength = 50;
}
