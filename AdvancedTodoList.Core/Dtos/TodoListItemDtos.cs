using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.Core.Dtos;

/// <summary>
/// DTO for creating/editing a to-do list item.
/// </summary>
public record TodoItemCreateDto(string Name, string Description, DateTime? DeadlineDate);

/// <summary>
/// DTO for changing the state of a to-do list item.
/// </summary>
public record TodoItemChangeStateDto(TodoItemState State);

/// <summary>
/// DTO for a full view of a to-do list.
/// </summary>
public record TodoItemGetByIdDto(
	string Id, string TodoListId, string Name, 
	string Description, DateTime? DeadlineDate,
	TodoItemState State
	);

/// <summary>
/// DTO for a partial view of a to-do list.
/// </summary>
public record TodoItemPreviewDto(
	string Id, string TodoListId, string Name,
	DateTime? DeadlineDate, TodoItemState State
	);