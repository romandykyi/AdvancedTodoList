using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Interface for a service that manages to-do list members.
/// </summary>
public interface ITodoListMembersService
{
	/// <summary>
	/// Gets a page with members of a to-do list asynchronously.
	/// </summary>
	/// <param name="todoListId">ID of the to-do list to add member to.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains a page of <see cref="TodoListMemberPreviewDto"/> objects or
	/// <see langword="null" /> if the to-do list does not exist.
	/// </returns>
	Task<Page<TodoListMemberPreviewDto>?> GetMembersAsync(string todoListId, PaginationParameters paginationParameters);

	/// <summary>
	/// Adds a member to a to-do list asynchronously.
	/// </summary>
	/// <remarks>
	/// Parameter <paramref name="dto"/> is not validated in this method.
	/// </remarks>
	/// <param name="todoListId">ID of the to-do list to add member to.</param>
	/// <param name="dto">DTO that contains information needed for adding a member. Supossed to be valid.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	Task<AddTodoListMemberResult> AddMemberAsync(string todoListId, TodoListMemberAddDto dto);

	/// <summary>
	/// Updates a role of the member of a to-do list asynchronously.
	/// </summary>
	/// <param name="todoListId">ID of the to-do list.</param>
	/// <param name="memberId">ID of the member.</param>
	/// <param name="dto">DTO that contains information needed for updating a role.</param>
	/// <returns>
	/// A task representing the asynchronous operation.
	/// If user's role was updated successfully than <see langword="true"/> is returned;
	/// otherwise <see langword="false" /> if the user or the to-do list was not found
	/// </returns>
	Task<bool> UpdateMemberRoleAsync(string todoListId, int memberId, TodoListMemberUpdateRoleDto dto);

	/// <summary>
	/// Removes a member from a to-do list asynchronously.
	/// </summary>
	/// <param name="todoListId">ID of the to-do list to remove member from.</param>
	/// <param name="memberId">ID of the member.</param>
	/// <returns>
	/// A task representing the asynchronous operation.
	/// If user was removed successfully than <see langword="true"/> is returned;
	/// otherwise <see langword="false" /> if the user or the to-do list was not found
	/// </returns>
	Task<bool> RemoveMemberAsync(string todoListId, int memberId);
}
