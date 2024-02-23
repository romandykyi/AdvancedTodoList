﻿using AdvancedTodoList.Core.Models.Auth;

namespace AdvancedTodoList.Core.Services.Auth;

/// <summary>
/// An interface for a service that manages access tokens.
/// </summary>
public interface IAccessTokensService
{
	/// <summary>
	/// Generates an access token for the user.
	/// </summary>
	/// <param name="user">User which will receive an access token.</param>
	/// <returns>
	/// A string that represents an access token.
	/// </returns>
	string GenerateAccessToken(ApplicationUser user);

	/// <summary>
	/// Validates an access token without checking expiration time and then returns 
	/// ID of the user stored in it asynchronously.
	/// </summary>
	/// <param name="accessToken">A string that represents an access token.</param>
	/// <returns>
	/// A user ID retrieved from the access token or <see langword="null" />, if validation failed.
	/// </returns>
	Task<string?> GetUserIdFromExpiredTokenAsync(string accessToken);
}
