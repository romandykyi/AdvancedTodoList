using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using Mapster;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists.
/// </summary>
public class TodoListsService(IRepository<TodoList, string> repository) : ITodoListsService
{
	private readonly IRepository<TodoList, string> _repository = repository;

	/// <summary>
	/// Retrieves a to-do list by its ID asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoListGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public async Task<TodoListGetByIdDto?> GetByIdAsync(string id)
	{
		var todoList = await _repository.GetByIdAsync(id);
		if (todoList == null) return null;

		return todoList.Adapt<TodoListGetByIdDto>();
	}

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <param name="dto">The DTO containing information for creating the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoList"/> mapped to 
	/// <see cref="TodoListCreateDto"/>.
	/// </returns>
	public async Task<TodoListGetByIdDto> CreateAsync(TodoListCreateDto dto)
	{
		// Map DTO to model
		var todoList = dto.Adapt<TodoList>();
		// Add model to the database
		await _repository.AddAsync(todoList);
		// Return DTO of created model
		return todoList.Adapt<TodoListGetByIdDto>();
	}

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
	public async Task<bool> EditAsync(string id, TodoListCreateDto dto)
	{
		// Get the model
		var todoList = await _repository.GetByIdAsync(id);
		// Return false if the model doesn't exist
		if (todoList == null) return false;

		// Update the model
		dto.Adapt(todoList);
		await _repository.UpdateAsync(todoList);

		return true;
	}

	/// <summary>
	/// Deletes a to-do list asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if entity was not found.
	/// </returns>
	public async Task<bool> DeleteAsync(string id)
	{
		// Get the model
		var todoList = await _repository.GetByIdAsync(id);
		// Return false if the model doesn't exist
		if (todoList == null) return false;

		// Delete the model
		await _repository.DeleteAsync(todoList);

		return true;
	}
}
