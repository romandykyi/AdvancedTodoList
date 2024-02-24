using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering any todo-list items.
/// </summary>
/// <param name="todoListId">ID of the list items of which will be obtained.</param>
public class TodoDependantEntitySpecification<TEntity>(string todoListId) : ISpecification<TEntity>
	where TEntity : ITodoListDependant
{
	private readonly Expression<Func<TEntity, bool>> _criteria = x => x.TodoListId == todoListId;

	public Expression<Func<TEntity, bool>> Criteria => _criteria;

	public List<Expression<Func<TEntity, object>>> Includes => [];

	public List<string> IncludeStrings => [];
}
