using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Interface for a service that manages to-do list items.
/// </summary>
public interface ITodoItemsService
{
	/// <summary>
	/// Retrieves a to-do list item by its ID asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoItemGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<TodoItemGetByIdDto?> GetByIdAsync(int id);

	/// <summary>
	/// Creates a new to-do list item asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list to associate the item with.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoItem"/>.
	/// </returns>
	public Task<TodoItem> CreateAsync(string todoListId, TodoItemCreateDto dto);

	/// <summary>
	/// Edits a to-do list item asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> EditAsync(string id, TodoItemCreateDto dto);

	/// <summary>
	/// Updates the state of a to-do list item asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to update the state.</param>
	/// <param name="dto">The DTO containing information for updating the state of the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> UpdateStateAsync(string id, TodoItemUpdateStateDto dto);

	/// <summary>
	/// Deletes a to-do list item asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> DeleteAsync(string id);
}
