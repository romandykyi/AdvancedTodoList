using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models;

public class TodoList
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public string Id { get; set; } = null!;

	public string Name { get; set; } = null!;

	public virtual IEnumerable<TodoItem> TodoItems { get; set; } = null!;
}
