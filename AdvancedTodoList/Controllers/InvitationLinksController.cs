using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Controllers;

[Authorize]
[ApiController]
[Route("api/todo/{listId}/invitationLinks")]
public class InvitationLinksController(IInvitationLinksService invitationLinksService) : ControllerBase
{
    private readonly IInvitationLinksService _invitationLinksService = invitationLinksService;

    /// <summary>
    /// Gets a page with invitation links of the to-do list with the specified ID.
    /// </summary>
    /// <param name="listId">ID of the to-do list.</param>
    /// <param name="paginationParameters">Paginations parameters to apply.</param>
    /// <response code="200">Returns invitation links of the to-do list.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list was not found.</response>
    [HttpGet(Name = nameof(GetInvitationLinksAsync))]
    [ProducesResponseType(typeof(Page<InvitationLinkDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitationLinksAsync(
        [FromRoute] string listId, [FromQuery] PaginationParameters paginationParameters)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _invitationLinksService.GetInvitationLinksAsync(context, paginationParameters);
        return result.ToActionResult();
    }

    /// <summary>
    /// Creates a new to-do list invitation link.
    /// </summary>
    /// <param name="listId">ID of the to-do list which will contain the invitation link.</param>
    /// <response code="200">Successfully created.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list was not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvitationLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostInvitationLinkAsync([FromRoute] string listId)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var response = await _invitationLinksService.CreateAsync(context);
        return response.ToActionResult();
    }

    /// <summary>
    /// Deletes a to-do list invitation link.
    /// </summary>
    /// <response code="204">Success.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="403">User has no permission to perform this action.</response>
    /// <response code="404">To-do list invitation link was not found.</response>
    [HttpDelete("{invitationLinkId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInvitationLinkAsync(
        [FromRoute] string listId, [FromRoute] int invitationLinkId)
    {
        TodoListContext context = new(listId, User.GetUserId()!);
        var result = await _invitationLinksService.DeleteAsync(context, invitationLinkId);
        return result.ToActionResult();
    }
}
