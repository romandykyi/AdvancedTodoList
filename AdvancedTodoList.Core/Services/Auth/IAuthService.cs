using AdvancedTodoList.Core.Dtos;

namespace AdvancedTodoList.Core.Services.Auth;

/// <summary>
/// Interface for service that performs authentication operations.
/// </summary>
public interface IAuthService
{
	/// <summary>
	/// Logs a user in asynchronously.
	/// </summary>
	/// <param name="logInDto">Data required for logging in.</param>
	/// <returns>
	/// Returns a task representing the asynchronous operation, containing a 
	/// response with access and refresh tokens or null if authorization fails.
	/// </returns>
	Task<LogInResponse?> LogInAsync(LogInDto logInDto);

	/// <summary>
	/// Registers a new user asynchronously.
	/// </summary>
	/// <param name="registerDto">Data required for user registration.</param>
	/// <returns>
	/// Returns a task representing the asynchronous operation, containing the registration result.
	/// </returns>
	Task<RegisterResult> RegisterAsync(RegisterDto registerDto);

	/// <summary>
	/// Refreshes the authentication token asynchronously.
	/// </summary>
	/// <param name="userId">ID of the caller.</param>
	/// <param name="refreshDto">Data required for token refresh.</param>
	/// <returns>
	/// Returns a task representing the asynchronous operation,
	/// containing a response with access and refresh tokens or null if authorization fails.
	/// </returns>
	Task<LogInResponse?> RefreshAsync(string userId, RefreshDto refreshDto);

	/// <summary>
	/// Logs a user out asynchronously by revoking a refresh token.
	/// </summary>
	/// <param name="userId">ID of the caller.</param>
	/// <param name="revokeDto">Data required for logging out.</param>
	/// <returns>
	/// Returns a task representing the asynchronous operation,
	/// indicating the success or failure of the operation.
	/// </returns>
	Task<bool> LogOutAsync(string userId, LogOutDto revokeDto);
}
