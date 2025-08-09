using AdvancedTodoList.Core.Models.TodoLists;
using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications.Todo;

/// <summary>
/// Represents a specification used for obtaining a to-do list aggregate.
/// </summary>
/// <param name="id">The unique identifier of the to-do list to obtain.</param>
public class TodoListAggregateSpecification(string id) : GetByIdSpecification<TodoList, string>(id)
{
    /// <summary>
    /// Gets the list of include expressions specifying an owner.
    /// </summary>
    public override List<Expression<Func<TodoList, object?>>> Includes =>
    [
        x => x.Owner
    ];
}
