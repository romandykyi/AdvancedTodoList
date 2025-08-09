using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace AdvancedTodoList.Application.Services.Implementations.Auth;

/// <summary>
/// Service that performs authentication operations.
/// </summary>
public class AuthService(
    IAccessTokensService accessTokensService,
    IRefreshTokensService refreshTokensService,
    UserManager<ApplicationUser> userManager)
    : IAuthService
{
    private readonly IAccessTokensService _accessTokensService = accessTokensService;
    private readonly IRefreshTokensService _refreshTokensService = refreshTokensService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    /// <summary>
    /// Logs a user in asynchronously.
    /// </summary>
    /// <param name="logInDto">Data required for logging in.</param>
    /// <returns>
    /// Returns a task representing the asynchronous operation, containing a 
    /// response with access and refresh tokens or null if authorization fails.
    /// </returns>
    public async Task<LogInResponse?> LogInAsync(LogInDto logInDto)
    {
        // Try to find a user
        ApplicationUser? user;
        // Try to find by email
        if (logInDto.UserNameOrEmail.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(logInDto.UserNameOrEmail);
        }
        // Try to find by username
        else
        {
            user = await _userManager.FindByNameAsync(logInDto.UserNameOrEmail);
        }

        // Invalid username/email - fail
        if (user == null) return null;

        // Invalid password - fail
        if (!await _userManager.CheckPasswordAsync(user, logInDto.Password))
            return null;

        // Generate a new refresh token for the user
        string? refreshToken = await _refreshTokensService.GenerateAsync(user.Id);
        if (refreshToken == null) return null;

        // Generate an access token
        string accessToken = _accessTokensService.GenerateAccessToken(user);

        // Return both tokens
        return new(accessToken, refreshToken);
    }

    /// <summary>
    /// Logs a user out asynchronously by revoking a refresh token.
    /// </summary>
    /// <param name="userId">ID of the caller.</param>
    /// <param name="logOutDto">Data required for logging out.</param>
    /// <returns>
    /// Returns a task representing the asynchronous operation,
    /// indicating the success or failure of the operation.
    /// </returns>
    public async Task<bool> LogOutAsync(string userId, LogOutDto logOutDto)
    {
        // Revoke the token
        return await _refreshTokensService.RevokeAsync(userId, logOutDto.RefreshToken);
    }

    /// <summary>
    /// Refreshes the access token asynchronously.
    /// </summary>
    /// <param name="refreshDto">Data required for token refresh.</param>
    /// <returns>
    /// Returns a task representing the asynchronous operation,
    /// containing a response with access and refresh tokens or null if authorization fails.
    /// </returns>
    public async Task<LogInResponse?> RefreshAsync(RefreshDto refreshDto)
    {
        // Try to get a user ID
        string? userId = await _accessTokensService
            .GetUserIdFromExpiredTokenAsync(refreshDto.AccessToken);
        if (userId == null) return null;

        // Validate the refresh token
        if (!await _refreshTokensService.ValidateAsync(userId, refreshDto.RefreshToken))
            return null;

        // Find the user
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        // User doesn't exist - return null
        if (user == null) return null;

        // Generate access token
        string accessToken = _accessTokensService.GenerateAccessToken(user);

        // Return tokens
        return new LogInResponse(accessToken, refreshDto.RefreshToken);
    }

    /// <summary>
    /// Registers a new user asynchronously.
    /// </summary>
    /// <param name="registerDto">Data required for user registration.</param>
    /// <returns>
    /// Returns a task representing the asynchronous operation, containing the registration result.
    /// </returns>
    public async Task<RegisterResult> RegisterAsync(RegisterDto registerDto)
    {
        List<RegisterError> errors = [];
        // Check if email is available
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            errors.Add(new("Email", "Email is already taken."));
        // Check if username is available
        if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
            errors.Add(new("UserName", "Username is already taken."));

        if (errors.Count > 0) return RegisterResult.Failure(errors);

        // Try to register the user
        ApplicationUser user = new()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.UserName
        };
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        // Return the result
        return result.Succeeded ?
            RegisterResult.Success() :
            RegisterResult.Failure(IdentityErrorsToRegisterErrors(result.Errors));
    }

    private static IEnumerable<RegisterError> IdentityErrorsToRegisterErrors(IEnumerable<IdentityError> identityErrors)
    {
        foreach (var error in identityErrors)
        {
            // Determine the property which caused an error
            string property = "$";
            if (error.Code.Contains("Password")) property = "Password";
            else if (error.Code.Contains("UserName")) property = "UserName";
            else if (error.Code.Contains("Email")) property = "Email";

            yield return new(property, error.Description);
        }
    }
}
