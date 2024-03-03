using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
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
	/// <param name="todoListId">The ID of the to-do list which roles will be retrieved.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains a page of <see cref="TodoListRolePreviewDto"/> objects or
	/// <see langword="null" /> if the to-do list does not exist.
	/// </returns>
	public Task<Page<TodoListRolePreviewDto>?> GetRolesOfListAsync(string todoListId, PaginationParameters paginationParameters)
	{
		TodoListDependantEntitiesSpecification<TodoListRole> specification = new(todoListId);
		return _helperService.GetPageAsync<TodoListRolePreviewDto>(todoListId, specification, paginationParameters);
	}

	/// <summary>
	/// Retrieves a to-do list role by its ID asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the role.</param>
	/// <param name="roleId">The ID of the to-do list role to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoListRoleViewDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<TodoListRoleViewDto?> GetByIdAsync(string todoListId, int roleId)
	{
		return _helperService.GetByIdAsync<TodoListRoleViewDto>(todoListId, roleId);
	}

	/// <summary>
	/// Creates a new to-do list role asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list to associate the role with.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list role.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoListRole"/> mapped to 
	/// <see cref="TodoListRoleViewDto"/> or <see langword="null" /> if to-do list with ID
	/// <paramref name="todoListId"/> does not exist.
	/// </returns>
	public Task<TodoListRoleViewDto?> CreateAsync(string todoListId, TodoListRoleCreateDto dto)
	{
		return _helperService.CreateAsync<TodoListRoleCreateDto, TodoListRoleViewDto>(todoListId, dto);
	}

	/// <summary>
	/// Edits a to-do list role asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the role.</param>
	/// <param name="roleId">The ID of the to-do list role to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list role.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> EditAsync(string todoListId, int roleId, TodoListRoleCreateDto dto)
	{
		return _helperService.UpdateAsync(todoListId, roleId, dto);
	}

	/// <summary>
	/// Deletes a to-do list role asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the role.</param>
	/// <param name="roleId">The ID of the to-do list role to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> DeleteAsync(string todoListId, int roleId)
	{
		return _helperService.DeleteAsync(todoListId, roleId);
	}
}
