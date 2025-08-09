namespace AdvancedTodoList.Application.Services.Definitions.Auth;

/// <summary>
/// An interface for a service that manages refresh tokens.
/// </summary>
public interface IRefreshTokensService
{
	/// <summary>
	/// Generates a refresh token for the user and saves it asynchronously.
	/// </summary>
	/// <param name="userId">ID of the user who will receive the token.</param>
	/// <returns>
	/// A string that represents a refresh token or <see langword="null" /> if user does not exist.
	/// </returns>
	Task<string?> GenerateAsync(string userId);

	/// <summary>
	/// Revokes the refresh token of the user asynchronously
	/// </summary>
	/// <param name="userId">ID of the user whose token is being revoked.</param>
	/// <param name="token">Value of the token to be revoked.</param>
	/// <returns>
	/// <see langword="true" /> on success; <see langword="false" /> otherwise.
	/// </returns>
	Task<bool> RevokeAsync(string userId, string token);

	/// <summary>
	/// Checks whether refresh token is valid asynchronously.
	/// </summary>
	/// <param name="userId">ID of the user whose token is being validated.</param>
	/// <param name="token">Value of the token to be validated.</param>
	/// <returns>
	/// <see langword="true" /> if token is valid;
	/// <see langword="false" /> otherwise.
	/// </returns>
	Task<bool> ValidateAsync(string userId, string token);
}
