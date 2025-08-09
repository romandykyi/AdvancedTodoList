using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications.Filters;
using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications.Todo;

/// <summary>
/// Represents a specification that defines criteria for filtering to-do lists,
/// with the requirement that the user is a member of those lists.
/// </summary>
/// <param name="userId">ID of the user.</param>
/// <param name="filter">Filter parameters.</param>
public class TodoListsSpecification(string userId, TodoListsFilter filter) : ISpecification<TodoList>
{
    /// <summary>
    /// Gets the ID of the user, to get lists where this user is a member.
    /// </summary>
    public string UserId { get; } = userId;
    /// <summary>
    /// Gets the filter parameters.
    /// </summary>
    public TodoListsFilter Filter { get; } = filter;

    /// <summary>
    /// Gets the criteria expression that defines the filtering conditions.
    /// </summary>
    public Expression<Func<TodoList, bool>> Criteria => x =>
        // User is a member requirement
        x.TodoListMembers.Any(m => m.UserId == UserId) &&
        // Filter by name
        (Filter.Name == null || x.Name.Contains(Filter.Name));

    /// <summary>
    /// Gets the list of include expressions specifying related entities to be included in the query results.
    /// </summary>
    public List<Expression<Func<TodoList, object?>>> Includes =>
    [
        x => x.TodoListMembers
    ];

    /// <summary>
    /// Gets the list of include strings specifying related entities to be included in the query results.
    /// </summary>
    public List<string> IncludeStrings => [];
}
