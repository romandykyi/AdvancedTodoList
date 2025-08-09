using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
	private readonly IAuthService _authService = authService;
	private readonly ILogger<AuthController> _logger = logger;

	/// <summary>
	/// Accepts user credentials and returns access and refresh tokens.
	/// </summary>
	/// <response code="200">Returns access and refresh tokens.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Invalid credentials.</response>
	[HttpPost("logIn")]
	[ProducesResponseType(typeof(LogInResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> LogInAsync([FromBody] LogInDto logInDto)
	{
		var result = await _authService.LogInAsync(logInDto);
		if (result == null) return Unauthorized();
		return Ok(result);
	}

	/// <summary>
	/// Registers a user.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
	{
		var result = await _authService.RegisterAsync(registerDto);
		if (result.IsSuccess) return NoContent();

		// Add errors to the model state
		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(error.Property, error.Message);
		}
		return BadRequest(ModelState);
	}

	/// <summary>
	/// Accepts an old access token with a valid refresh token and returns a new access token.
	/// </summary>
	/// <response code="200">Returns access and refresh tokens.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Invalid access/refresh token.</response>
	[HttpPost("refresh")]
	[ProducesResponseType(typeof(LogInResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> RefreshAsync([FromBody] RefreshDto refreshDto)
	{
		var result = await _authService.RefreshAsync(refreshDto);
		if (result == null) return Unauthorized();

		return Ok(result);
	}

	/// <summary>
	/// Revokes specified refresh token for the current user.
	/// </summary>
	/// <response code="204">Success.</response>
	/// <response code="400">Validation failed.</response>
	/// <response code="401">Authorization failed/invalid refresh token.</response>
	[Authorize]
	[HttpPost("logOut")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> LogOutAsync([FromBody] LogOutDto logOutDto)
	{
		string? userId = User.GetUserId();
		if (userId == null)
		{
			_logger.LogWarning("User ID could not be found in claims.");
			return Unauthorized();
		}
		return await _authService.LogOutAsync(userId, logOutDto) ?
			NoContent() : Unauthorized();
	}
}
