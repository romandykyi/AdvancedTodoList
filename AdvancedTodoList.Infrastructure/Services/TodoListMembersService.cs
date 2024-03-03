using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do list members.
/// </summary>
public class TodoListMembersService(
	ITodoListDependantEntitiesService<TodoListMember, int> helperService,
	ITodoListMembersRepository repository) :
	ITodoListMembersService
{
	private readonly ITodoListDependantEntitiesService<TodoListMember, int> _helperService = helperService;
	private readonly ITodoListMembersRepository _repository = repository;

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
	public Task<Page<TodoListMemberPreviewDto>?> GetMembersAsync(string todoListId, PaginationParameters paginationParameters)
	{
		TodoListMembersSpecification specification = new(todoListId);
		return _helperService.GetPageAsync<TodoListMemberPreviewDto>(
			todoListId, specification, paginationParameters
			);
	}

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
	public async Task<TodoListMemberServiceResult> AddMemberAsync(string todoListId, TodoListMemberAddDto dto)
	{
		// Try to find already existing member
		var member = await _repository.FindAsync(todoListId, dto.UserId);
		// Return error if it exists
		if (member != null) return new(TodoListMemberServiceResultStatus.UserAlreadyAdded);

		// Add member
		var outputDto = await _helperService
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(todoListId, dto);
		if (outputDto == null) return new(TodoListMemberServiceResultStatus.NotFound);

		return new(TodoListMemberServiceResultStatus.Success, outputDto);
	}

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
	public async Task<TodoListMemberServiceResult> UpdateMemberRoleAsync(string todoListId, int memberId, TodoListMemberUpdateRoleDto dto)
	{
		// Checks if role is valid for the action should be implemented here

		bool updated = await _helperService.UpdateAsync(todoListId, memberId, dto);
		return new(updated ? TodoListMemberServiceResultStatus.Success :
			TodoListMemberServiceResultStatus.NotFound);
	}

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
	public Task<bool> RemoveMemberAsync(string todoListId, int memberId)
	{
		return _helperService.DeleteAsync(todoListId, memberId);
	}
}
