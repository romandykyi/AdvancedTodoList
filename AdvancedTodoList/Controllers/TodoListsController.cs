using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications.Filters;
using AdvancedTodoList.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Controllers;

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
    /// Gets a page with to-do lists of which the caller is a member.
    /// </summary>
    /// <param name="paginationParameters">Paginations parameters to apply.</param>
    /// <param name="filter">Filter parameters to apply.</param>
    /// <response code="200">Returns to-do lists of which the caller is a member.</response>
    /// <response code="401">Authentication failed.</response>
    [HttpGet(Name = nameof(GetTodoListsOfCallerAsync))]
    [ProducesResponseType(typeof(Page<TodoListPreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTodoListsOfCallerAsync(
        [FromQuery] PaginationParameters paginationParameters, [FromQuery] TodoListsFilter filter)
    {
        var page = await _todoListsService.GetListsOfUserAsync(User.GetUserId()!,
            paginationParameters, filter);
        return Ok(page);
    }

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
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _todoListsService.GetByIdAsync(context);
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
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _todoListsService.EditAsync(context, dto);
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
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _todoListsService.DeleteAsync(context);
        return result.ToActionResult();
    }
}
