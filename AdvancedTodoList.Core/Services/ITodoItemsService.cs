﻿using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Interface for a service that manages to-do list items.
/// </summary>
public interface ITodoItemsService
{
	/// <summary>
	/// Retrieves to-do list items of the list with the specified ID.
	/// </summary>
	/// <remarks>
	/// Does not throw exceptions if ID is invalid.
	/// </remarks>
	/// <param name="id">The ID of the to-do list which items will be retrieved.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains a collection of <see cref="TodoItemPreviewDto"/> objects.
	/// </returns>
	public Task<IEnumerable<TodoItemPreviewDto>> GetItemsOfListAsync(string id);

	/// <summary>
	/// Retrieves a to-do list ID of the to-do list item.
	/// </summary>
	/// <param name="id">ID of the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// an ID of the to-do list which owns a to-do list item with the specified ID if it's found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<string?> GetTodoListByIdAsync(int id);

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
	public Task<bool> EditAsync(int id, TodoItemCreateDto dto);

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
	public Task<bool> UpdateStateAsync(int id, TodoItemUpdateStateDto dto);

	/// <summary>
	/// Deletes a to-do list item asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public Task<bool> DeleteAsync(int id);
}
