using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Controllers;

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
    /// <param name="name">Optional name to filter roles by.</param>
    /// <response code="200">Returns roles of the to-do list.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list was not found.</response>
    [HttpGet(Name = nameof(GetTodoListRolesAsync))]
    [ProducesResponseType(typeof(Page<TodoListRolePreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTodoListRolesAsync(
        [FromRoute] string listId, [FromQuery] PaginationParameters paginationParameters,
        [FromQuery] string? name)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _rolesService.GetRolesOfListAsync(context, paginationParameters, name);
        return response.ToActionResult();
    }

    /// <summary>
    /// Gets a to-do list role by its ID.
    /// </summary>
    /// <param name="listId">ID of the to-do list which contans the role to obtain.</param>
    /// <param name="roleId">ID of the to-do list role to obtain.</param>
    /// <response code="200">Returns requested to-do list role.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list role was not found.</response>
    [HttpGet("{roleId}", Name = nameof(GetTodoListRoleByIdAsync))]
    [ProducesResponseType(typeof(TodoListRoleViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTodoListRoleByIdAsync(
        [FromRoute] string listId, [FromRoute] int roleId)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _rolesService.GetByIdAsync(context, roleId);
        return response.ToActionResult();
    }

    /// <summary>
    /// Creates a new to-do list role.
    /// </summary>
    /// <param name="listId">ID of the to-do list which will contain the role.</param>
    /// <param name="dto"></param>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list was not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TodoListRoleViewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostTodoListRoleAsync(
        [FromRoute] string listId, [FromBody] TodoListRoleCreateDto dto)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _rolesService.CreateAsync(context, dto);
        if (response.Status != ServiceResponseStatus.Success || response.Result == null)
            return response.ToActionResult();

        var routeValues = new { listId, roleId = response.Result.Id };
        return CreatedAtRoute(nameof(GetTodoListRoleByIdAsync), routeValues, response.Result);
    }

    /// <summary>
    /// Updates a to-do list role.
    /// </summary>
    /// <response code="204">Success.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list role was not found.</response>
    [HttpPut("{roleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutTodoListRoleAsync(
        [FromRoute] string listId, [FromRoute] int roleId,
        [FromBody] TodoListRoleCreateDto dto)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _rolesService.EditAsync(context, roleId, dto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deletes a to-do list role.
    /// </summary>
    /// <response code="204">Success.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list role was not found.</response>
    [HttpDelete("{roleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTodoListAsync(
        [FromRoute] string listId, [FromRoute] int roleId)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _rolesService.DeleteAsync(context, roleId);
        return result.ToActionResult();
    }
}
