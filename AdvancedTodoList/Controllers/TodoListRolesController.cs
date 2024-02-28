using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.Controllers;

[Authorize]
[ApiController]
[Route("api/todo/{listId}/roles")]
public class TodoListRolesController(ITodoListRolesService rolesService) : ControllerBase
{
	private readonly ITodoListRolesService _rolesService = rolesService;

	/// <summary>
	/// Gets a page with roles of the to-do list with the specified ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list.</param>
	/// <param name="paginationParameters">Paginations parameters to apply.</param>
	/// <response code="200">Returns roles of the to-do list.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpGet(Name = nameof(GetTodoListRolesAsync))]
	[ProducesResponseType(typeof(Page<TodoListRolePreviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoListRolesAsync(
		[FromRoute] string listId, [FromQuery] PaginationParameters paginationParameters)
	{
		var page = await _rolesService.GetRolesOfListAsync(listId, paginationParameters);
		if (page == null) return NotFound();
		return Ok(page);
	}

	/// <summary>
	/// Gets a to-do list role by its ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list which contans the role to obtain.</param>
	/// <param name="roleId">ID of the to-do list role to obtain.</param>
	/// <response code="200">Returns requested to-do list role.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list role was not found.</response>
	[HttpGet("{roleId}", Name = nameof(GetTodoListRoleByIdAsync))]
	[ProducesResponseType(typeof(TodoListRoleViewDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoListRoleByIdAsync(
		[FromRoute] string listId, [FromRoute] int roleId)
	{
		var role = await _rolesService.GetByIdAsync(listId, roleId);
		// Check if role exists
		if (role == null) return NotFound();

		return Ok(role);
	}

	/// <summary>
	/// Creates a new to-do list role.
	/// </summary>
	/// <param name="listId">ID of the to-do list which will contain the role.</param>
	/// <param name="dto"></param>
	/// <response code="201">Successfully created.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpPost]
	[ProducesResponseType(typeof(TodoListRoleViewDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PostTodoListRoleAsync(
		[FromRoute] string listId, [FromBody] TodoListRoleCreateDto dto)
	{
		var role = await _rolesService.CreateAsync(listId, dto);
		if (role == null) return NotFound();

		var routeValues = new { listId, roleId = role.Id };
		return CreatedAtRoute(nameof(GetTodoListRoleByIdAsync), routeValues, role);
	}

	/// <summary>
	/// Updates a to-do list role.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list role was not found.</response>
	[HttpPut("{roleId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> PutTodoListRoleAsync(
		[FromRoute] string listId, [FromRoute] int roleId,
		[FromBody] TodoListRoleCreateDto dto)
	{
		bool result = await _rolesService.EditAsync(listId, roleId, dto);
		return result ? NoContent() : NotFound();
	}

	/// <summary>
	/// Deletes a to-do list role.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list role was not found.</response>
	[HttpDelete("{roleId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteTodoListAsync(
		[FromRoute] string listId, [FromRoute] int roleId)
	{
		bool result = await _rolesService.DeleteAsync(listId, roleId);
		return result ? NoContent() : NotFound();
	}
}
