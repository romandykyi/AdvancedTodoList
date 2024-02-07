using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models;

namespace AdvancedTodoList.Core.Services;

public interface ITodoListsService
{
	/// <summary>
	/// Retrieves a to-do list by its unique identifier asynchronously.
	/// </summary>
	/// <param name="id">The unique identifier of the to-do list to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoListGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<TodoListGetByIdDto?> GetByIdAsync(string id);

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <param name="dto">The data transfer object containing information for creating the todo list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoList"/>.
	/// </returns>
	public Task<TodoList> CreateAsync(TodoListCreateDto dto);
}
