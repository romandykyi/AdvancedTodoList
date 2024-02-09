using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Services;

public class TodoListsService(ApplicationDbContext dbContext) : ITodoListsService
{
	private readonly ApplicationDbContext _dbContext = dbContext;

	/// <summary>
	/// Retrieves a to-do list by its unique identifier asynchronously.
	/// </summary>
	/// <param name="id">The unique identifier of the todo list to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoListGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public async Task<TodoListGetByIdDto?> GetByIdAsync(string id)
	{
		var list = await _dbContext.TodoLists
			.Include(x => x.TodoItems)
			.AsNoTracking()
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync();
		// Todo list does not exist - return null
		if (list == null) return null;

		return list.Adapt<TodoListGetByIdDto>();
	}

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <param name="dto">The data transfer object containing information for creating the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoList"/>.
	/// </returns>
	public async Task<TodoList> CreateAsync(TodoListCreateDto dto)
	{
		var list = dto.Adapt<TodoList>();
		_dbContext.Add(list);
		await _dbContext.SaveChangesAsync();
		return list;
	}

	public Task<bool> EditAsync(string id, TodoListCreateDto dto)
	{
		throw new NotImplementedException();
	}

	public Task<bool> DeleteAsync(string id)
	{
		throw new NotImplementedException();
	}
}
