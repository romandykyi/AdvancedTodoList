using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Interface for a service that manages to-do list role.
/// </summary>
public interface ITodoListRolesService
{
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
	public Task<Page<TodoListRolePreviewDto>?> GetRolesOfListAsync(string todoListId, PaginationParameters paginationParameters);

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
	public Task<TodoListRoleViewDto?> GetByIdAsync(string todoListId, int roleId);

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
	public Task<TodoListRoleViewDto?> CreateAsync(string todoListId, TodoListRoleCreateDto dto);

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
	public Task<bool> EditAsync(string todoListId, int roleId, TodoListRoleCreateDto dto);

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
	public Task<bool> DeleteAsync(string todoListId, int roleId);
}
