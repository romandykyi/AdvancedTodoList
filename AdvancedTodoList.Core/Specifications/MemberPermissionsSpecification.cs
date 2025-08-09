using AdvancedTodoList.Core.Models.TodoLists.Members;
using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications;

/// <summary>
/// Represents a specification for obtaining an aggregate containg the to-do list member
/// and his/her/their role, used in <see cref="PermissionsChecker" />.
/// </summary>
/// <param name="todoListId">ID of the to-do list.</param>
/// <param name="userId">ID of the user.</param>
public class MemberPermissionsSpecification(string todoListId, string userId) : ISpecification<TodoListMember>
{
    /// <summary>
    /// Gets the to-do list ID.
    /// </summary>
    public string TodoListId { get; } = todoListId;
    /// <summary>
    /// Gets the user ID.
    /// </summary>
    public string UserId { get; } = userId;

    /// <summary>
    /// Gets the criteria expression that defines the filtering conditions.
    /// </summary>
    public Expression<Func<TodoListMember, bool>> Criteria =>
        x => x.TodoListId == TodoListId && x.UserId == UserId;

    /// <summary>
    /// Gets the list of include expressions specifying a to-do list role to be included in the query results.
    /// </summary>
    public List<Expression<Func<TodoListMember, object?>>> Includes =>
    [
        x => x.Role
    ];

    /// <summary>
    /// Gets the list of include strings specifying related entities to be included in the query results.
    /// </summary>
    public List<string> IncludeStrings { get; } = [];
}
