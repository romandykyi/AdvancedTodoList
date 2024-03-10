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
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	Task<ServiceResponse<Page<TodoListMemberPreviewDto>>> GetMembersAsync(TodoListContext context, PaginationParameters paginationParameters);

	/// <summary>
	/// Adds a member to a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">DTO that contains information needed for adding a member. Supossed to be valid.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	Task<AddTodoListMemberServiceResult> AddMemberAsync(TodoListContext context, TodoListMemberAddDto dto);

	/// <summary>
	/// Updates a role of the member of a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="memberId">ID of the member.</param>
	/// <param name="dto">DTO that contains information needed for updating a role.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	Task<TodoListMemberServiceResultStatus> UpdateMemberRoleAsync(TodoListContext context,
		int memberId, TodoListMemberUpdateRoleDto dto);

	/// <summary>
	/// Removes a member from a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="memberId">ID of the member.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	Task<ServiceResponseStatus> RemoveMemberAsync(TodoListContext context, int memberId);
}
