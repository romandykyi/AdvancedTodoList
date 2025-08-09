using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Controllers;

[Authorize]
[ApiController]
[Route("api/todo/join")]
public class JoinTodoListController(IInvitationLinksService invitationLinksService) : ControllerBase
{
	private readonly IInvitationLinksService _invitationLinksService = invitationLinksService;

	/// <summary>
	/// Uses the invitation link to join the caller to a to-do list.
	/// </summary>
	/// <param name="invitationLinkValue">Value of the invitation link.</param>
	/// <response code="204">Successfully joined.</response>
	/// <response code="401">Authentication failed.</response>
	/// <response code="404">Link was not found.</response>
	/// <response code="410">Link is expired.</response>
	/// <response code="422">Caller is already the member of the to-do list.</response>
	[HttpPost("{invitationLinkValue}")]
	[ProducesResponseType(typeof(TodoListMemberMinimalViewDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status410Gone)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	public async Task<IActionResult> JoinByInvitationLinkAsync([FromRoute] string invitationLinkValue)
	{
		var response = await _invitationLinksService.JoinAsync(User.GetUserId()!, invitationLinkValue);

		return response.Status switch
		{
			JoinByInvitationLinkStatus.Success => Ok(response.Dto),
			JoinByInvitationLinkStatus.NotFound => NotFound(),
			JoinByInvitationLinkStatus.Expired => Problem(
				statusCode: StatusCodes.Status410Gone, detail: "Invitation link is expired."),
			JoinByInvitationLinkStatus.UserIsAlreadyMember => Problem(
				statusCode: StatusCodes.Status422UnprocessableEntity, detail: "Caller is already the member of the to-do list."),
			_ => throw new InvalidOperationException("Invalid invitation links service response.")
		};
	}
}
