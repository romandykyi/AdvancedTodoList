using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists roles.
/// </summary>
public class TodoListRolesService(ITodoListDependantEntitiesService<TodoListRole, int> helperService) : ITodoListRolesService
{
	private readonly ITodoListDependantEntitiesService<TodoListRole, int> _helperService = helperService;

	/// <summary>
	/// Retrieves a page of to-do list roles of the list with the specified ID.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<Page<TodoListRolePreviewDto>>> GetRolesOfListAsync(TodoListContext context, PaginationParameters paginationParameters)
	{
		TodoListDependantEntitiesSpecification<TodoListRole> specification = new(context.TodoListId);
		return _helperService.GetPageAsync<TodoListRolePreviewDto>(context, specification, paginationParameters);
	}

	/// <summary>
	/// Retrieves a to-do list role by its ID asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="roleId">The ID of the to-do list role to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<TodoListRoleViewDto>> GetByIdAsync(TodoListContext context, int roleId)
	{
		return _helperService.GetByIdAsync<TodoListRoleViewDto>(context, roleId);
	}

	/// <summary>
	/// Creates a new to-do list role asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list role.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<TodoListRoleViewDto>> CreateAsync(TodoListContext context, TodoListRoleCreateDto dto)
	{
		return _helperService.CreateAsync<TodoListRoleCreateDto, TodoListRoleViewDto>(
			context, dto, x => x.EditRoles);
	}

	/// <summary>
	/// Edits a to-do list role asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="roleId">The ID of the to-do list role to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list role.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> EditAsync(TodoListContext context, int roleId, TodoListRoleCreateDto dto)
	{
		return _helperService.UpdateAsync(context, roleId, dto, x => x.EditRoles);
	}

	/// <summary>
	/// Deletes a to-do list role asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="roleId">The ID of the to-do list role to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int roleId)
	{
		return _helperService.DeleteAsync(context, roleId, x => x.EditRoles);
	}
}
