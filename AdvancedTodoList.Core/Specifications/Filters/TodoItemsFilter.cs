using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.Core.Specifications.Filters;

/// <summary>
/// Parameters for filtering to-do lists items.
/// </summary>
/// <param name="Name">
/// Optional name to filter by. 
/// Entries which have this substring in the name will be returned.
/// </param>
/// <param name="State">Optional states to filter by.</param>
/// <param name="OwnerId">Optional ID of the owner to filter by.</param>
/// <param name="CategoryId">Optional categories to filter by.</param>
/// <param name="MinPriority">Optional min priority to filter by.</param>
/// <param name="MaxPriority">Optional max priority to filter by.</param>
/// <param name="MinDeadlineDate">Optional min deadline date to filter by.</param>
/// <param name="MaxDeadlineDate">Optional max deadline date to filter by.</param>
public record TodoItemsFilter(
    string? Name = null, string? OwnerId = null,
    IEnumerable<TodoItemState>? State = null, IEnumerable<int?>? CategoryId = null,
    int? MinPriority = null, int? MaxPriority = null,
    DateTime? MinDeadlineDate = null, DateTime? MaxDeadlineDate = null);
