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
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<Page<TodoListMemberPreviewDto>>> GetMembersAsync(TodoListContext context, PaginationParameters paginationParameters)
	{
		TodoListMembersSpecification specification = new(context.TodoListId);
		return _helperService.GetPageAsync<TodoListMemberPreviewDto>(
			context, specification, paginationParameters
			);
	}

	/// <summary>
	/// Adds a member to a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">DTO that contains information needed for adding a member. Supossed to be valid.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public async Task<AddTodoListMemberServiceResult> AddMemberAsync(TodoListContext context, TodoListMemberAddDto dto)
	{
		// Try to find already existing member
		var member = await _repository.FindAsync(context.TodoListId, dto.UserId);
		// Return error if it exists
		if (member != null) return new(TodoListMemberServiceResultStatus.UserAlreadyAdded);

		// Add member
		var response = await _helperService
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(context, dto, x => x.AddMembers);

		return response.Status switch
		{
			ServiceResponseStatus.Success => new(TodoListMemberServiceResultStatus.Success, response.Result),
			ServiceResponseStatus.NotFound => new(TodoListMemberServiceResultStatus.NotFound),
			ServiceResponseStatus.Forbidden => new(TodoListMemberServiceResultStatus.Forbidden),
			_ => throw new InvalidOperationException("Invalid to-do lists dependant entities (members) service response.")
		};
	}

	/// <summary>
	/// Updates a role of the member of a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="memberId">ID of the member.</param>
	/// <param name="dto">DTO that contains information needed for updating a role.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<TodoListMemberServiceResultStatus> UpdateMemberRoleAsync(TodoListContext context, int memberId, TodoListMemberUpdateRoleDto dto)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Removes a member from a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="memberId">ID of the member.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> RemoveMemberAsync(TodoListContext context, int memberId)
	{
		return _helperService.DeleteAsync(context, memberId, x => x.RemoveMembers);
	}
}
