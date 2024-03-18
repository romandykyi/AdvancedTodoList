using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering to-do list members
/// and adds includes for users and roles models.
/// </summary>
/// <param name="todoListId">ID of the to-do list items of which will be obtained.</param>
/// <param name="filter">Filter parameters.</param>
public class TodoListMembersSpecification(string todoListId, TodoListMembersFilter filter) : ISpecification<TodoListMember>
{
	/// <summary>
	/// Gets the ID of the to-do list to filter entities by.
	/// </summary>
	public string? TodoListId { get; } = todoListId;
	/// <summary>
	/// Gets the filter parameters.
	/// </summary>
	public TodoListMembersFilter Filter { get; } = filter;

	/// <summary>
	/// Gets the criteria expression that defines the filtering conditions.
	/// </summary>
	public Expression<Func<TodoListMember, bool>> Criteria =>
		x => x.TodoListId == TodoListId &&
		// Filter by roles
		(Filter.RoleId == null || !Filter.RoleId.Any() || Filter.RoleId.Contains(x.RoleId)) &&
		// Filter by user ID
		(Filter.UserId == null || x.UserId == Filter.UserId) &&
		// Filter by username
		(Filter.UserName == null || x.User!.UserName!.Contains(Filter.UserName)) &&
		// Filter by full name
		(Filter.FullName == null || (x.User!.FirstName + ' ' + x.User!.LastName).Contains(Filter.FullName));

	/// <summary>
	/// Gets the list of include expressions specifying related entities to be included in the query results.
	/// </summary>
	public List<Expression<Func<TodoListMember, object?>>> Includes =>
	[
		x => x.Role,
		x => x.User
	];

	/// <summary>
	/// Gets the list of include strings specifying related entities to be included in the query results.
	/// </summary>
	public List<string> IncludeStrings => [];
}
