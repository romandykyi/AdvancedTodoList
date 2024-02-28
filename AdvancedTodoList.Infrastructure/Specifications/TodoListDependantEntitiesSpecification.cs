using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering any todo-list dependant entities.
/// </summary>
/// <param name="todoListId">ID of the list items of which will be obtained.</param>
public class TodoListDependantEntitiesSpecification<TEntity>(string todoListId) : ISpecification<TEntity>
	where TEntity : ITodoListDependant
{
	/// <summary>
	/// Gets the ID of the to-do list to filter entities by.
	/// </summary>
	public string? TodoListId { get; } = todoListId;

	/// <summary>
	/// Gets the criteria expression that defines the filtering conditions.
	/// Filters only by the to-do list ID when not overriden.
	/// </summary>
	public virtual Expression<Func<TEntity, bool>> Criteria => x => x.TodoListId == TodoListId;

	/// <summary>
	/// Gets the list of include expressions specifying related entities to be included in the query results.
	/// Is empty when not overriden.
	/// </summary>
	public virtual List<Expression<Func<TEntity, object?>>> Includes => [];

	/// <summary>
	/// Gets the list of include strings specifying related entities to be included in the query results.
	/// Is empty when not overriden.
	/// </summary>
	public virtual List<string> IncludeStrings => [];
}
