using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Interface for a service that manages to-do list items.
/// </summary>
public interface ITodoItemsService
{
	/// <summary>
	/// Retrieves a page of to-do list items of the list with the specified ID.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which items will be retrieved.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains a page of <see cref="TodoItemPreviewDto"/> objects or
	/// <see langword="null" /> if the to-do list does not exist.
	/// </returns>
	public Task<Page<TodoItemPreviewDto>?> GetItemsOfListAsync(string todoListId, PaginationParameters paginationParameters);

	/// <summary>
	/// Retrieves a to-do list item by its ID asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the item.</param>
	/// <param name="itemId">The ID of the to-do list item to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoItemGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<TodoItemGetByIdDto?> GetByIdAsync(string todoListId, int itemId);

	/// <summary>
	/// Creates a new to-do list item asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list to associate the item with.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoItem"/> mapped to 
	/// <see cref="TodoItemGetByIdDto"/> or <see langword="null" /> if to-do list with ID
	/// <paramref name="todoListId"/> does not exist.
	/// </returns>
	public Task<TodoItemGetByIdDto?> CreateAsync(string todoListId, TodoItemCreateDto dto);

	/// <summary>
	/// Edits a to-do list item asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the item.</param>
	/// <param name="itemId">The ID of the to-do list item to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> EditAsync(string todoListId, int itemId, TodoItemCreateDto dto);

	/// <summary>
	/// Updates the state of a to-do list item asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the item.</param>
	/// <param name="itemId">The ID of the to-do list item to update the state.</param>
	/// <param name="stateDto">The DTO which contains the state of the to-do item to set.</param></param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> UpdateStateAsync(string todoListId, int itemId, TodoItemUpdateStateDto stateDto);

	/// <summary>
	/// Deletes a to-do list item asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the item.</param>
	/// <param name="itemId">The ID of the to-do list item to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> DeleteAsync(string todoListId, int itemId);
}
