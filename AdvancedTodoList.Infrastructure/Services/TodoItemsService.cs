using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists items.
/// </summary>
public class TodoItemsService(ApplicationDbContext dbContext) : ITodoItemsService
{
	private readonly ApplicationDbContext _dbContext = dbContext;

	/// <summary>
	/// Retrieves a to-do list ID of the to-do list item.
	/// </summary>
	/// <param name="id">ID of the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// an ID of the to-do list which owns a to-do list item with the specified ID if it's found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public async Task<string?> GetTodoListByIdAsync(int id)
	{
		return await _dbContext.TodoItems
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(x => x.TodoListId)
			.FirstOrDefaultAsync();
	}

	/// <summary>
	/// Retrieves a to-do list item by its ID asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoItemGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public async Task<TodoItemGetByIdDto?> GetByIdAsync(int id)
	{
		return await _dbContext.TodoItems
			.AsNoTracking()
			.Where(x => x.Id == id)
			.ProjectToType<TodoItemGetByIdDto>()
			.FirstOrDefaultAsync();
	}

	/// <summary>
	/// Creates a new to-do list item asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list to associate the item with.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoItem"/>.
	/// </returns>
	public async Task<TodoItem> CreateAsync(string todoListId, TodoItemCreateDto dto)
	{
		var item = dto.Adapt<TodoItem>();
		_dbContext.TodoItems.Add(item);
		await _dbContext.SaveChangesAsync();
		return item;
	}

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
	public async Task<bool> EditAsync(int id, TodoItemCreateDto dto)
	{
		var item = await _dbContext.TodoItems
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// To-do item does not exist - return false
		if (item == null) return false;

		dto.Adapt(item);
		await _dbContext.SaveChangesAsync();

		return true;
	}

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
	public async Task<bool> UpdateStateAsync(int id, TodoItemUpdateStateDto dto)
	{
		var item = await _dbContext.TodoItems
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// To-do item does not exist - return false
		if (item == null) return false;

		item.State = dto.State;
		await _dbContext.SaveChangesAsync();

		return true;
	}

	/// <summary>
	/// Deletes a to-do list item asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list item to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public async Task<bool> DeleteAsync(int id)
	{
		var item = await _dbContext.TodoItems
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// To-do item does not exist - return false
		if (item == null) return false;

		_dbContext.TodoItems.Remove(item);
		await _dbContext.SaveChangesAsync();

		return true;
	}
}
