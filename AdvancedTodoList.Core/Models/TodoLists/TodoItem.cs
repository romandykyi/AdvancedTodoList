using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.TodoLists;

/// <summary>
/// Represents a to-do list item entity.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// An unique identifier for the to-do list item.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Name (title) of the to-do item.
    /// </summary>
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    /// <summary>
    /// Description of the to-do item.
    /// </summary>
    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;
    /// <summary>
    /// Current state of the to-do item.
    /// </summary>
    public TodoItemState State { get; set; }
	/// <summary>
	/// Deadline date for the todo item. Can be null.
	/// </summary>
	public DateTime? DeadlineDate { get; set; }

	/// <summary>
	/// Foreign key referencing the associated to-do list.
	/// </summary>
	[ForeignKey(nameof(TodoList))]
	public string TodoListId { get; set; } = null!;

	/// <summary>
	/// Maximum allowed length of <see cref="Name"/>.
	/// </summary>
	public const int NameMaxLength = 100;
    /// <summary>
    /// Maximum allowed length of <see cref="Description"/>.
    /// </summary>
    public const int DescriptionMaxLength = 10_000;

    /// <summary>
    /// To-do list associated with this to-do item.
    /// </summary>
    public TodoList TodoList { get; set; } = null!;
}

/// <summary>
/// An enum that represents the possible states of a to-do list item.
/// </summary>
public enum TodoItemState : byte
{
	/// <summary>
	/// The task is active (default state).
	/// </summary>
	Active = 0,
	/// <summary>
	/// The task has been completed.
	/// </summary>
	Completed,
	/// <summary>
	/// The task has been skipped.
	/// </summary>
	Skipped
}
