using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.Controllers;

[Authorize]
[ApiController]
[Route("api/todo")]
public class TodoListsController(
	ITodoListsService todoListsService,
	ILogger<TodoListsController> logger) : ControllerBase
{
	private readonly ITodoListsService _todoListsService = todoListsService;
	private readonly ILogger<TodoListsController> _logger = logger;

	/// <summary>
	/// Gets a to-do list by its ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list to obtain.</param>
	/// <response code="200">Returns requested to-do list.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpGet("{listId}", Name = nameof(GetTodoListByIdAsync))]
	[ProducesResponseType(typeof(TodoListGetByIdDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoListByIdAsync([FromRoute] string listId)
	{
		var result = await _todoListsService.GetByIdAsync(listId, User.GetUserId()!);
		return result.ToActionResult();
	}

	/// <summary>
	/// Creates a new to-do list.
	/// </summary>
	/// <response code="201">Successfully created.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	[HttpPost]
	[ProducesResponseType(typeof(TodoListGetByIdDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> PostTodoListAsync([FromBody] TodoListCreateDto dto)
	{
		var list = await _todoListsService.CreateAsync(dto, User.GetUserId()!);
		var routeValues = new { listId = list.Id };
		return CreatedAtRoute(nameof(GetTodoListByIdAsync), routeValues, list);
	}

	/// <summary>
	/// Updates a to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpPut("{listId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PutTodoListAsync(
		[FromRoute] string listId, [FromBody] TodoListCreateDto dto)
	{
		var result = await _todoListsService.EditAsync(listId, dto, User.GetUserId()!);
		return result.ToActionResult();
	}

	/// <summary>
	/// Deletes a to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpDelete("{listId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteTodoListAsync([FromRoute] string listId)
	{
		var result = await _todoListsService.DeleteAsync(listId, User.GetUserId()!);
		return result.ToActionResult();
	}
}
