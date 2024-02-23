using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering todo-list items.
/// </summary>
/// <param name="todoListId">ID of the list items of which will be obtained.</param>
public class TodoItemsSpecification(string todoListId) : ISpecification<TodoItem>
{
	private readonly Expression<Func<TodoItem, bool>> _criteria = x => x.TodoListId == todoListId;

	public Expression<Func<TodoItem, bool>> Criteria => _criteria;

	public List<Expression<Func<TodoItem, object>>> Includes => [];

	public List<string> IncludeStrings => [];
}
