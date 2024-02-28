using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.Controllers;

[Authorize]
[ApiController]
[Route("api/todo/{listId}/items")]
public class TodoItemsController(ITodoItemsService todoItemsService) : ControllerBase
{
	private readonly ITodoItemsService _rolesService = todoItemsService;

	/// <summary>
	/// Gets a page with items of the to-do list with the specified ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list.</param>
	/// <param name="paginationParameters">Paginations parameters to apply.</param>
	/// <response code="200">Returns items of the to-do list.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpGet(Name = nameof(GetListItemsAsync))]
	[ProducesResponseType(typeof(Page<TodoItemPreviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetListItemsAsync(
		[FromRoute] string listId, [FromQuery] PaginationParameters paginationParameters)
	{
		var page = await _rolesService.GetItemsOfListAsync(listId, paginationParameters);
		if (page == null) return NotFound();
		return Ok(page);
	}

	/// <summary>
	/// Gets a to-do list item by its ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list which contans the item to obtain.</param>
	/// <param name="itemId">ID of the to-do list item to obtain.</param>
	/// <response code="200">Returns requested to-do list item.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list item was not found.</response>
	[HttpGet("{itemId}", Name = nameof(GetTodoItemByIdAsync))]
	[ProducesResponseType(typeof(TodoItemGetByIdDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoItemByIdAsync(
		[FromRoute] string listId, [FromRoute] int itemId)
	{
		var item = await _rolesService.GetByIdAsync(listId, itemId);
		// Check if item exists
		if (item == null) return NotFound();

		return Ok(item);
	}

	/// <summary>
	/// Creates a new to-do list item.
	/// </summary>
	/// <param name="listId">ID of the to-do list which will contain the item.</param>
	/// <param name="dto"></param>
	/// <response code="201">Successfully created.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpPost]
	[ProducesResponseType(typeof(TodoItemGetByIdDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PostTodoItemAsync(
		[FromRoute] string listId, [FromBody] TodoItemCreateDto dto)
	{
		var item = await _rolesService.CreateAsync(listId, dto);
		if (item == null) return NotFound();

		var routeValues = new { listId, itemId = item.Id };
		return CreatedAtRoute(nameof(GetTodoItemByIdAsync), routeValues, item);
	}

	/// <summary>
	/// Updates a to-do list item.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list item was not found.</response>
	[HttpPut("{itemId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PutTodoItemAsync(
		[FromRoute] string listId, [FromRoute] int itemId,
		[FromBody] TodoItemCreateDto dto)
	{
		bool result = await _rolesService.EditAsync(listId, itemId, dto);
		return result ? NoContent() : NotFound();
	}

	/// <summary>
	/// Updates a to-do list item's state.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list item was not found.</response>
	[HttpPut("{itemId}/state")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PutTodoItemStateAsync(
		[FromRoute] string listId, [FromRoute] int itemId,
		[FromBody] TodoItemUpdateStateDto dto)
	{
		bool result = await _rolesService.UpdateStateAsync(listId, itemId, dto);
		return result ? NoContent() : NotFound();
	}

	/// <summary>
	/// Deletes a to-do list item.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list item was not found.</response>
	[HttpDelete("{itemId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteTodoListAsync(
		[FromRoute] string listId, [FromRoute] int itemId)
	{
		bool result = await _rolesService.DeleteAsync(listId, itemId);
		return result ? NoContent() : NotFound();
	}
}
