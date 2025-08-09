using AdvancedTodoList.Core.Models.TodoLists;
using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications.Todo;

/// <summary>
/// Represents a specification used for obtaining a to-do list item aggregate.
/// </summary>
/// <param name="id">The unique identifier of the to-do list item to obtain.</param>
public class TodoItemAggregateSpecification(int id) : GetByIdSpecification<TodoItem, int>(id)
{
    /// <summary>
    /// Gets the list of include expressions specifying an owner.
    /// </summary>
    public override List<Expression<Func<TodoItem, object?>>> Includes =>
    [
        x => x.Owner,
        x => x.Category
    ];
}
