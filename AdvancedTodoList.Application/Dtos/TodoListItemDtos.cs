using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.Application.Dtos;

/// <summary>
/// DTO for creating/editing a to-do list item.
/// </summary>
public record TodoItemCreateDto(
	string Name, string Description, DateTime? DeadlineDate,
	int Priority, int? CategoryId
	);

/// <summary>
/// DTO for changing the state of a to-do list item.
/// </summary>
public record TodoItemUpdateStateDto(TodoItemState State);

/// <summary>
/// DTO for a full view of a to-do list item.
/// </summary>
public record TodoItemGetByIdDto(
	int Id, string TodoListId, string Name,
	string Description, DateTime? DeadlineDate,
	TodoItemState State, int Priority,
	ApplicationUserPreviewDto Owner,
	TodoItemCategoryViewDto? Category
	);

/// <summary>
/// DTO for a partial view of a to-do list item.
/// </summary>
public record TodoItemPreviewDto(
	int Id, string TodoListId, string Name,
	DateTime? DeadlineDate, TodoItemState State,
	int Priority, ApplicationUserPreviewDto Owner,
	TodoItemCategoryViewDto? Category
	);