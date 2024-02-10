using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists.
/// </summary>
public class TodoListsService(ApplicationDbContext dbContext) : ITodoListsService
{
	private readonly ApplicationDbContext _dbContext = dbContext;

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
		var list = await _dbContext.TodoLists
			.AsNoTracking()
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// To-do list does not exist - return null
		if (list == null) return null;

		return list.Adapt<TodoListGetByIdDto>();
	}

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <param name="dto">The DTO containing information for creating the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoList"/>.
	/// </returns>
	public async Task<TodoList> CreateAsync(TodoListCreateDto dto)
	{
		var list = dto.Adapt<TodoList>();
		_dbContext.TodoLists.Add(list);
		await _dbContext.SaveChangesAsync();
		return list;
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
		var entity = await _dbContext.TodoLists
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// To-do list does not exist - return false
		if (entity == null) return false;

		dto.Adapt(entity);
		await _dbContext.SaveChangesAsync();

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
		var entity = await _dbContext.TodoLists
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// To-do list does not exist - return false
		if (entity == null) return false;

		_dbContext.TodoLists.Remove(entity);
		await _dbContext.SaveChangesAsync();

		return true;
	}
}
