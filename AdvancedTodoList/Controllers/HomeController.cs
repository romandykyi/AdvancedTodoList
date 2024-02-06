using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Controllers;

[ApiController]
[Route("home")]
public class HomeController(ApplicationDbContext dbContext) : ControllerBase
{
	private readonly ApplicationDbContext _dbContext = dbContext;

	public record TodoListAdd(string Name, string[] Items);

	[HttpGet]
	public async Task<ActionResult<IEnumerable<TodoList>>> Get()
	{
		return await _dbContext.TodoLists.ToListAsync();
	}

	[HttpGet]
	[Route("{listId}/items")]
	public async Task<ActionResult<IEnumerable<string>>> GetItems(string listId)
	{
		TodoList? list = await _dbContext.TodoLists
			.Include(x => x.TodoItems)
			.Where(x => x.Id == listId)
			.FirstOrDefaultAsync();
		if (list == null)
		{
			return NotFound();
		}

		return Ok(list.TodoItems.Select(x => x.Name));
	}

	[HttpPost]
	public async Task<ActionResult> Post(TodoListAdd todoList)
	{
		TodoList list = new()
		{
			Name = todoList.Name
		};
		_dbContext.Add(list);
		await _dbContext.SaveChangesAsync();

		foreach (string itemName in todoList.Items)
		{
			TodoItem item = new()
			{
				Name = itemName,
				TodoListId = list.Id
			};
			_dbContext.Add(item);
		}
		await _dbContext.SaveChangesAsync();

		return Ok();
	}
}
