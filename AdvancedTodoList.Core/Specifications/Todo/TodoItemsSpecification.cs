using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications.Filters;
using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications.Todo;

/// <summary>
/// Represents a specification that defines criteria for filtering to-do list members
/// and adds includes for users and roles models.
/// </summary>
/// <param name="todoListId">ID of the to-do list items of which will be obtained.</param>
/// <param name="filter">Filter parameters.</param>
public class TodoItemsSpecification(string todoListId, TodoItemsFilter filter) :
    ISpecification<TodoItem>
{
    /// <summary>
    /// Gets the ID of the to-do list to filter items by.
    /// </summary>
    public string? TodoListId { get; } = todoListId;
    /// <summary>
    /// Gets the filter parameters.
    /// </summary>
    public TodoItemsFilter Filter { get; } = filter;

    /// <summary>
    /// Gets the criteria expression that defines the filtering conditions.
    /// </summary>
    public Expression<Func<TodoItem, bool>> Criteria => x =>
        // Filter by to-do list
        x.TodoListId == TodoListId &&
        // Filter by name
        (Filter.Name == null || x.Name.Contains(Filter.Name)) &&
        // Filter by owner's ID
        (Filter.OwnerId == null || x.OwnerId == Filter.OwnerId) &&
        // Filter by states
        (Filter.State == null || !Filter.State.Any() || Filter.State.Contains(x.State)) &&
        // Filter by categories
        (Filter.CategoryId == null || !Filter.CategoryId.Any() || Filter.CategoryId.Contains(x.CategoryId)) &&
        // Filter by priority
        (Filter.MinPriority == null || x.Priority >= Filter.MinPriority) &&
        (Filter.MaxPriority == null || x.Priority <= Filter.MaxPriority) &&
        // Filter by deadline date
        (Filter.MinDeadlineDate == null || x.DeadlineDate >= Filter.MinDeadlineDate) &&
        (Filter.MaxDeadlineDate == null || x.DeadlineDate <= Filter.MaxDeadlineDate);

    /// <summary>
    /// Gets the list of include expressions specifying related entities to be included in the query results.
    /// </summary>
    public List<Expression<Func<TodoItem, object?>>> Includes =>
    [
        x => x.Owner,
        x => x.Category
    ];

    /// <summary>
    /// Gets the list of include strings specifying related entities to be included in the query results.
    /// </summary>
    public List<string> IncludeStrings => [];
}
