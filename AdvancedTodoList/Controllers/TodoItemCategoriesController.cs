using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Controllers;

[Authorize]
[ApiController]
[Route("api/todo/{listId}/categories")]
public class TodoItemCategoriesController(ITodoItemCategoriesService categoriesService) :
    ControllerBase
{
    private readonly ITodoItemCategoriesService _categoriesService = categoriesService;

    /// <summary>
    /// Gets a page with categories of the to-do list with the specified ID.
    /// </summary>
    /// <param name="listId">ID of the to-do list.</param>
    /// <param name="paginationParameters">Paginations parameters to apply.</param>
    /// <param name="name">Optional name to filter roles by.</param>
    /// <response code="200">Returns categories of the to-do list.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list was not found.</response>
    [HttpGet(Name = nameof(GetTodoListCategoriesAsync))]
    [ProducesResponseType(typeof(Page<TodoListRolePreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTodoListCategoriesAsync(
        [FromRoute] string listId, [FromQuery] PaginationParameters paginationParameters,
        [FromQuery] string? name)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _categoriesService.GetCategoriesOfListAsync(context, paginationParameters, name);
        return response.ToActionResult();
    }

    /// <summary>
    /// Gets a to-do list category by its ID.
    /// </summary>
    /// <param name="listId">ID of the to-do list which contans the category to obtain.</param>
    /// <param name="categoryId">ID of the to-do list category to obtain.</param>
    /// <response code="200">Returns requested to-do list category.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list category was not found.</response>
    [HttpGet("{categoryId}", Name = nameof(GetTodoListCategoryByIdAsync))]
    [ProducesResponseType(typeof(TodoListRoleViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTodoListCategoryByIdAsync(
        [FromRoute] string listId, [FromRoute] int categoryId)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _categoriesService.GetByIdAsync(context, categoryId);
        return response.ToActionResult();
    }

    /// <summary>
    /// Creates a new to-do list category.
    /// </summary>
    /// <param name="listId">ID of the to-do list which will contain the category.</param>
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
    public async Task<IActionResult> PostTodoItemCategoryAsync(
        [FromRoute] string listId, [FromBody] TodoItemCategoryCreateDto dto)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _categoriesService.CreateAsync(context, dto);
        if (response.Status != ServiceResponseStatus.Success || response.Result == null)
            return response.ToActionResult();

        var routeValues = new { listId, categoryId = response.Result.Id };
        return CreatedAtRoute(nameof(GetTodoListCategoryByIdAsync), routeValues, response.Result);
    }

    /// <summary>
    /// Updates a to-do list category.
    /// </summary>
    /// <response code="204">Success.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list category was not found.</response>
    [HttpPut("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutTodoItemCategoryAsync(
        [FromRoute] string listId, [FromRoute] int categoryId,
        [FromBody] TodoItemCategoryCreateDto dto)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _categoriesService.EditAsync(context, categoryId, dto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deletes a to-do list category.
    /// </summary>
    /// <response code="204">Success.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list category was not found.</response>
    [HttpDelete("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTodoItemCategoryAsync(
        [FromRoute] string listId, [FromRoute] int categoryId)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _categoriesService.DeleteAsync(context, categoryId);
        return result.ToActionResult();
    }
}
