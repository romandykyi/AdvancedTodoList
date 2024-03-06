using AdvancedTodoList.Core.Dtos;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Interface for a service that manages to-do lists.
/// </summary>
public interface ITodoListsService
{
	/// <summary>
	/// Retrieves a to-do list by its ID asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to retrieve.</param>
	/// <param name="callerId">ID of the user who calls the operation.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponse<TodoListGetByIdDto>> GetByIdAsync(string id, string callerId);

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
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list.</param>
	/// <param name="callerId">ID of the user who calls the operation.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponseStatus> EditAsync(string id, TodoListCreateDto dto, string callerId);

	/// <summary>
	/// Deletes a to-do list asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <param name="callerId">ID of the user who calls the operation.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponseStatus> DeleteAsync(string id, string callerId);
}
