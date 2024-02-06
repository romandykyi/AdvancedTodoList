using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models;

public class TodoItem
{
	[Key]
	public int Id { get; set; }
	public string Name { get; set; } = null!;

	[ForeignKey(nameof(TodoList))]
	public string TodoListId { get; set; } = null!;

	public TodoList TodoList { get; set; } = null!;
}
