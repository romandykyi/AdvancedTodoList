namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Represents a context of an operation related to the to-do list.
/// </summary>
/// <param name="TodoListId">ID of the to-do list.</param>
/// <param name="CallerId">ID of the caller.</param>
public record TodoListContext(string TodoListId, string CallerId);
