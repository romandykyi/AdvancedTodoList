using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Infrastructure.Specifications;
using Mapster;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists roles.
/// </summary>
public class TodoListRolesService(
	ITodoListDependantEntitiesService<TodoListRole, int> helperService,
	IRepository<TodoListRole, int> rolesRepository,
	IPermissionsChecker permissionsChecker
	) : ITodoListRolesService
{
	private readonly ITodoListDependantEntitiesService<TodoListRole, int> _helperService = helperService;
	private readonly IRepository<TodoListRole, int> _rolesRepository = rolesRepository;
	private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;

	/// <summary>
	/// Retrieves a page of to-do list roles of the list with the specified ID.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <param name="name">Optional name to filter categories by.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<Page<TodoListRolePreviewDto>>> GetRolesOfListAsync(
		TodoListContext context, PaginationParameters paginationParameters, string? name = null)
	{
		TodoListDependantEntitiesSpecification<TodoListRole> specification = new(context.TodoListId, name);
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
	/// It's not practically possible for this implementation to have the result that indicates 
	/// 'NotFound' status.
	/// </returns>
	public async Task<ServiceResponse<TodoListRoleViewDto>> CreateAsync(TodoListContext context, TodoListRoleCreateDto dto)
	{
		// Check if user has a permission to create a role with the given priority
		if (!await _permissionsChecker.HasPermissionOverRoleAsync(context, dto.Priority, x => x.EditRoles))
			return new(ServiceResponseStatus.Forbidden);

		// Pass null as the third argument to not check permissions twice
		return await _helperService.CreateAsync<TodoListRoleCreateDto, TodoListRoleViewDto>(
			context, dto, null);
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
	public async Task<ServiceResponseStatus> EditAsync(TodoListContext context, int roleId, TodoListRoleCreateDto dto)
	{
		// Get the model of a role
		var role = await _rolesRepository.GetByIdAsync(roleId);
		// Check if it's valid
		if (role == null || role.TodoListId != context.TodoListId)
			return ServiceResponseStatus.NotFound;

		// Check if user has a permission to change it
		// Checking only the minimal priority is sufficient, because both
		// values should be greater than caller's role priority.
		int minPriority = Math.Min(role.Priority, dto.Priority);
		if (!await _permissionsChecker.HasPermissionOverRoleAsync(context, minPriority, x => x.EditRoles))
			return ServiceResponseStatus.Forbidden;

		// Update the model
		dto.Adapt(role);
		// Save changes
		await _rolesRepository.UpdateAsync(role);

		return ServiceResponseStatus.Success;
	}

	/// <summary>
	/// Deletes a to-do list role asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="roleId">The ID of the to-do list role to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public async Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int roleId)
	{
		// Get the model of a role
		var role = await _rolesRepository.GetByIdAsync(roleId);
		// Check if it's valid
		if (role == null || role.TodoListId != context.TodoListId)
			return ServiceResponseStatus.NotFound;

		// Check if user has the permission to delete the role
		if (!await _permissionsChecker.HasPermissionOverRoleAsync(context, role.Priority, x => x.EditRoles))
			return ServiceResponseStatus.Forbidden;

		// Delete the role
		await _rolesRepository.DeleteAsync(role);

		return ServiceResponseStatus.Success;
	}
}
