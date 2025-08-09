namespace AdvancedTodoList.Core.Specifications.Filters;

/// <summary>
/// Parameters for filtering to-do lists.
/// </summary>
/// <param name="Name">
/// Optional name to filter by. 
/// Entries which have this substring in the name will be returned.
/// </param>
public record TodoListsFilter(string? Name = null);
