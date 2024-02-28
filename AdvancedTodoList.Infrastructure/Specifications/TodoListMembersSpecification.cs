using AdvancedTodoList.Core.Models.TodoLists.Members;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering to-do list members
/// and adds includes for users and roles models.
/// </summary>
/// <param name="todoListId">ID of the list items of which will be obtained.</param>
public class TodoListMembersSpecification(string todoListId) :
	TodoListDependantEntitiesSpecification<TodoListMember>(todoListId)
{
	/// <summary>
	/// Gets includes for role and user.
	/// </summary>
	public override List<Expression<Func<TodoListMember, object?>>> Includes =>
	[
		x => x.Role,
		x => x.User
	];
}
