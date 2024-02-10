using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoListsController(ITodoListsService todoListsService) : ControllerBase
{
	private readonly ITodoListsService _todoListsService = todoListsService;

	/// <summary>
	/// Gets a to-do list by its ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list to obtain.</param>
	/// <response code="200">Returns requested to-do list.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpGet("{listId}", Name = nameof(GetTodoListByIdAsync))]
	[ProducesResponseType(typeof(TodoListGetByIdDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoListByIdAsync(string listId)
	{
		var list = await _todoListsService.GetByIdAsync(listId);
		return list != null ? Ok(list) : NotFound();
	}

	/// <summary>
	/// Creates a new to-do list.
	/// </summary>
	/// <response code="201">Successfully created.</response>
	[HttpPost]
	[ProducesResponseType(typeof(TodoListGetByIdDto), StatusCodes.Status201Created)]
	public async Task<IActionResult> PostTodoListAsync([FromBody] TodoListCreateDto dto)
	{
		var list = await _todoListsService.CreateAsync(dto);
		var routeValues = new { listId = list.Id };
		var body = list.Adapt<TodoListGetByIdDto>();
		return CreatedAtRoute(nameof(GetTodoListByIdAsync), routeValues, body);
	}

	/// <summary>
	/// Updates a to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="204">To-do list was not found.</response>
	[HttpPut("{listId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PostTodoListAsync(
		[FromRoute] string listId, [FromBody] TodoListCreateDto dto)
	{
		bool result = await _todoListsService.EditAsync(listId, dto);
		return result ? NoContent() : NotFound();
	}

	/// <summary>
	/// Deletes a to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="204">To-do list was not found.</response>
	[HttpDelete("{listId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteTodoListAsync([FromRoute] string listId)
	{
		bool result = await _todoListsService.DeleteAsync(listId);
		return result ? NoContent() : NotFound();
	}
}
