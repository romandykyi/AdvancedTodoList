﻿using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.Controllers;

[Authorize]
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
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpGet("{listId}", Name = nameof(GetTodoListByIdAsync))]
	[ProducesResponseType(typeof(TodoListGetByIdDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoListByIdAsync([FromRoute] string listId)
	{
		var list = await _todoListsService.GetByIdAsync(listId);
		return list != null ? Ok(list) : NotFound();
	}

	/// <summary>
	/// Creates a new to-do list.
	/// </summary>
	/// <response code="201">Successfully created.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	[HttpPost]
	[ProducesResponseType(typeof(TodoListGetByIdDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> PostTodoListAsync([FromBody] TodoListCreateDto dto)
	{
		var list = await _todoListsService.CreateAsync(dto);
		var routeValues = new { listId = list.Id };
		return CreatedAtRoute(nameof(GetTodoListByIdAsync), routeValues, list);
	}

	/// <summary>
	/// Updates a to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpPut("{listId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PutTodoListAsync(
		[FromRoute] string listId, [FromBody] TodoListCreateDto dto)
	{
		bool result = await _todoListsService.EditAsync(listId, dto);
		return result ? NoContent() : NotFound();
	}

	/// <summary>
	/// Deletes a to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpDelete("{listId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteTodoListAsync([FromRoute] string listId)
	{
		bool result = await _todoListsService.DeleteAsync(listId);
		return result ? NoContent() : NotFound();
	}
}