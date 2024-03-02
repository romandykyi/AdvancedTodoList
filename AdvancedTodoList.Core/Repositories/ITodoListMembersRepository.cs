using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists.Members;

namespace AdvancedTodoList.Core.Repositories;

/// <summary>
/// Represents a repository interface for CRUD operations on to-do lists members.
/// </summary>
public interface ITodoListMembersRepository : IRepository<TodoListMember, int>
{
	/// <summary>
	/// Finds a to-do list member by to-do list ID and user's ID asynchronously.
	/// </summary>
	/// <param name="userId">ID of the user.</param>
	/// <param name="todoListId">ID of the to-do list.</param>
	/// <returns>
	/// Found to-do list member, or <see langword="null" /> if it was not found.
	/// </returns>
	Task<TodoListMember?> FindAsync(string todoListId, string userId);
}
