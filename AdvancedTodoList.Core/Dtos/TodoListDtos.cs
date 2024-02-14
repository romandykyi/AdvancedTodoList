namespace AdvancedTodoList.Core.Dtos;

/// <summary>
/// DTO for creating/editing a to-do list.
/// </summary>
public record TodoListCreateDto(string Name, string Description);

/// <summary>
/// DTO for a full view of a to-do list.
/// </summary>
public record TodoListGetByIdDto(string Id, string Name, string Description);
