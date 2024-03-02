using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;

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
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoListGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<TodoListGetByIdDto?> GetByIdAsync(string id);

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <param name="dto">The DTO containing information for creating the to-do list.</param>
	/// <param name="callerId">ID of the user who creates the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoList"/> mapped to 
	/// <see cref="TodoListGetByIdDto"/>.
	/// </returns>
	public Task<TodoListGetByIdDto> CreateAsync(TodoListCreateDto dto, string callerId);

	/// <summary>
	/// Edits a to-do list asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if entity was not found.
	/// </returns>
	public Task<bool> EditAsync(string id, TodoListCreateDto dto);

	/// <summary>
	/// Deletes a to-do list asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if entity was not found.
	/// </returns>
	public Task<bool> DeleteAsync(string id);
}
