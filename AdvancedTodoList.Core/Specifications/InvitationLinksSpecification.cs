using AdvancedTodoList.Core.Models.TodoLists;
using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering invitation links.
/// </summary>
/// <param name="todoListId">ID of the list invitation links of which will be obtained.</param>
public class InvitationLinksSpecification(string todoListId) : ISpecification<InvitationLink>
{
    /// <summary>
    /// Gets the ID of the to-do list to filter entities by.
    /// </summary>
    public string? TodoListId { get; } = todoListId;

    /// <summary>
    /// Gets the criteria expression that defines the filtering conditions.
    /// </summary>
    public virtual Expression<Func<InvitationLink, bool>> Criteria => x => x.TodoListId == TodoListId;

    /// <summary>
    /// Gets the list of include expressions specifying related entities to be included in the query results.
    /// </summary>
    public virtual List<Expression<Func<InvitationLink, object?>>> Includes { get; init; } = [];

    /// <summary>
    /// Gets the list of include strings specifying related entities to be included in the query results.
    /// </summary>
    public virtual List<string> IncludeStrings { get; init; } = [];
}
