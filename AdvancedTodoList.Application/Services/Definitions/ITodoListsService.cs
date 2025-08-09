using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications.Filters;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Interface for a service that manages to-do lists.
/// </summary>
public interface ITodoListsService
{
	/// <summary>
	/// Retrieves a page of to-do lists, with the requirement that the user 
	/// is a member of those lists.
	/// </summary>
	/// <param name="userId">Id of the user</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <param name="filter">Filter parameters to apply.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<Page<TodoListPreviewDto>> GetListsOfUserAsync(string userId,
		PaginationParameters paginationParameters, TodoListsFilter filter);

	/// <summary>
	/// Retrieves a to-do list by its ID asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponse<TodoListGetByIdDto>> GetByIdAsync(TodoListContext context);

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <remarks>
	/// This method should also create an "Owner" role with all permissions and assign the caller to it.
	/// </remarks>
	/// <param name="dto">The DTO containing information for creating the to-do list.</param>
	/// <param name="callerId">ID of the user who creates the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a created model mapped to <see cref="TodoListGetByIdDto" />.
	/// </returns>
	public Task<TodoListGetByIdDto> CreateAsync(TodoListCreateDto dto, string callerId);

	/// <summary>
	/// Edits a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponseStatus> EditAsync(TodoListContext context, TodoListCreateDto dto);

	/// <summary>
	/// Deletes a to-do list asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context);
}
