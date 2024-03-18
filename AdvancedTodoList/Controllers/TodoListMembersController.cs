using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.Controllers;

[Authorize]
[ApiController]
[Route("api/todo/{listId}/members")]
public class TodoListMembersController(
	ITodoListMembersService membersService,
	ILogger<TodoListMembersController> logger) : ControllerBase
{
	private readonly ITodoListMembersService _membersService = membersService;
	private readonly ILogger<TodoListMembersController> _logger = logger;

	/// <summary>
	/// Gets a page with members of the to-do list with the specified ID.
	/// </summary>
	/// <param name="listId">ID of the to-do list.</param>
	/// <param name="paginationParameters">Paginations parameters to apply.</param>
	/// <param name="filter">Filter parameters to use.</param>
	/// <response code="200">Returns members of the to-do list.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpGet(Name = nameof(GetTodoListMembersAsync))]
	[ProducesResponseType(typeof(Page<TodoListMemberPreviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTodoListMembersAsync(
		[FromRoute] string listId, [FromQuery] PaginationParameters paginationParameters,
		[FromQuery] TodoListMembersFilter filter)
	{
		TodoListContext context = new(listId, User.GetUserId()!);
		var response = await _membersService.GetMembersAsync(context, paginationParameters, filter);
		return response.ToActionResult();
	}

	/// <summary>
	/// Adds a member to the to-do list.
	/// </summary>
	/// <param name="listId">ID of the to-do list which will contain the member.</param>
	/// <param name="dto"></param>
	/// <response code="201">Successfully created.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	/// <response code="404">To-do list was not found.</response>
	[HttpPost]
	[ProducesResponseType(typeof(TodoListMemberMinimalViewDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AddMemberAsync(
		[FromRoute] string listId, [FromBody] TodoListMemberAddDto dto)
	{
		TodoListContext context = new(listId, User.GetUserId()!);
		var result = await _membersService.AddMemberAsync(context, dto);
		switch (result.Status)
		{
			case TodoListMemberServiceResultStatus.Success:
				return NoContent();
			case TodoListMemberServiceResultStatus.NotFound:
				return NotFound();
			case TodoListMemberServiceResultStatus.UserAlreadyAdded:
				ModelState.AddModelError("UserId", "The user is already a member of the to-do list.");
				return BadRequest(ModelState);
			case TodoListMemberServiceResultStatus.Forbidden:
				return Forbid();
		}
		_logger.LogError("Unexpected result status from to-do list members service when trying to add a member.");
		return StatusCode(StatusCodes.Status500InternalServerError);
	}

	/// <summary>
	/// Sets a role to the to-do list member.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="403">User has no permission to perform this action.</response>
	/// <response code="404">To-do list member was not found.</response>
	[HttpPut("{memberId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateMemberRoleAsync(
		[FromRoute] string listId, [FromRoute] int memberId,
		[FromBody] TodoListMemberUpdateRoleDto dto)
	{
		TodoListContext context = new(listId, User.GetUserId()!);
		var result = await _membersService.UpdateMemberRoleAsync(context, memberId, dto);
		switch (result)
		{
			case TodoListMemberServiceResultStatus.Success:
				return NoContent();
			case TodoListMemberServiceResultStatus.NotFound:
				return NotFound();
			case TodoListMemberServiceResultStatus.InvalidRoleId:
				ModelState.AddModelError("RoleId", "The role ID is invalid for the current to-do list.");
				return BadRequest(ModelState);
			case TodoListMemberServiceResultStatus.Forbidden:
				return Forbid();
		}
		_logger.LogError("Unexpected result status from to-do list members service when trying to update member's role.");
		return StatusCode(StatusCodes.Status500InternalServerError);
	}

	/// <summary>
	/// Removes a member from the to-do list.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">To-do list member was not found.</response>
	[HttpDelete("{memberId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> RemoveMemberAsync(
		[FromRoute] string listId, [FromRoute] int memberId)
	{
		TodoListContext context = new(listId, User.GetUserId()!);
		var result = await _membersService.RemoveMemberAsync(context, memberId);
		return result.ToActionResult();
	}
}
