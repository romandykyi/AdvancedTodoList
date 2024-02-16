namespace AdvancedTodoList.Core.Dtos;

/// <summary>
/// DTO for log in.
/// </summary>
public record LogInDto(string UserNameOrEmail, string Password);

/// <summary>
/// Response of the log in and the refresh endpoints.
/// </summary>
public record LogInResponse(string AccessToken, int ExpirationSeconds, string RefreshToken);

/// <summary>
/// DTO for register.
/// </summary>
public record RegisterDto(string Email, string UserName, string FirstName, string LastName, string Password);

/// <summary>
/// DTO for refreshing access token.
/// </summary>
public record RefreshDto(string AccessToken, string RefreshToken);

/// <summary>
/// DTO for revoking a refresh token.
/// </summary>
public record LogOutDto(string RefreshToken);
