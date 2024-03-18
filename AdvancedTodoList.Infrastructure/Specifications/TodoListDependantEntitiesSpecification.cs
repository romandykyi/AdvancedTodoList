using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering any todo-list dependant entities with name.
/// </summary>
/// <param name="todoListId">ID of the list items of which will be obtained.</param>
/// <param name="name">
/// Optional name to filter by. 
/// Entries which have this substring in the name will be returned.
/// </param>
public class TodoListDependantEntitiesSpecification<TEntity>(string todoListId, string? name = null) : ISpecification<TEntity>
	where TEntity : ITodoListDependant, IHasName
{
	/// <summary>
	/// Gets the ID of the to-do list to filter entities by.
	/// </summary>
	public string? TodoListId { get; } = todoListId;
	/// <summary>
	/// Gets the name to filter entities, which implement <see cref="IHasName" /> by.
	/// </summary>
	public string? Name { get; } = name;

	/// <summary>
	/// Gets the criteria expression that defines the filtering conditions.
	/// Filters only by the to-do list ID and the name when not overriden.
	/// </summary>
	public virtual Expression<Func<TEntity, bool>> Criteria => x => 
		x.TodoListId == TodoListId && (Name == null || x.Name.Contains(Name));

	/// <summary>
	/// Gets the list of include expressions specifying related entities to be included in the query results.
	/// Is empty when not overriden.
	/// </summary>
	public virtual List<Expression<Func<TEntity, object?>>> Includes { get; init; } = [];

	/// <summary>
	/// Gets the list of include strings specifying related entities to be included in the query results.
	/// Is empty when not overriden.
	/// </summary>
	public virtual List<string> IncludeStrings { get; init; } = [];
}
