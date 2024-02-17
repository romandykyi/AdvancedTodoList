using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AdvancedTodoList.Infrastructure.Services.Auth;

/// <summary>
/// Service that performs authentication operations.
/// </summary>
public class AuthService(
	ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
	IConfiguration configuration)
	: IAuthService
{
	private readonly ApplicationDbContext _dbContext = dbContext;
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly IConfiguration _configuration = configuration;

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
		{
			return null;
		}

		// Generate a new refresh token for the user
		string refreshToken = await GenerateRefreshTokenAsync(user);

		// Generate an access token and return the response
		return GetLogInResponse(user, refreshToken);
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
		// Try to find a refresh token in the DB
		UserRefreshToken? refreshToken = await _dbContext.UserRefreshTokens
			.Where(x => x.UserId == userId && x.Token == logOutDto.RefreshToken)
			.FirstOrDefaultAsync();
		// Token does not exist - fail
		if (refreshToken == null) return false;

		// Delete the token
		_dbContext.UserRefreshTokens.Remove(refreshToken);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	/// <summary>
	/// Refreshes the authentication token asynchronously.
	/// </summary>
	/// <param name="refreshDto">Data required for token refresh.</param>
	/// <returns>
	/// Returns a task representing the asynchronous operation,
	/// containing a response with access and refresh tokens or null if authorization fails.
	/// </returns>
	public async Task<LogInResponse?> RefreshAsync(RefreshDto refreshDto)
	{
		// Try to get a user ID
		string? userId = await GetUserIdFromExpiredTokenAsync(refreshDto.AccessToken);
		if (userId == null) return null;

		// Validate the refresh token
		if (!await ValidateRefreshTokenAsync(userId, refreshDto.RefreshToken)) return null;

		// Find the user
		ApplicationUser? user = await _userManager.FindByIdAsync(userId);
		// User doesn't exist - return null
		if (user == null) return null;

		// Return access token
		return GetLogInResponse(user, refreshDto.RefreshToken);
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

	// Generates JWT token and returns a response, requires refresh token
	private LogInResponse GetLogInResponse(ApplicationUser user, string refreshToken)
	{
		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["Auth:SecretKey"]!));
		int expirationSeconds = _configuration.GetValue<int>("Auth:AccessTokenExpirationSeconds");

		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(JwtRegisteredClaimNames.Email, user.Email!),
			new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
			new(JwtRegisteredClaimNames.GivenName, user.FirstName),
			new(JwtRegisteredClaimNames.FamilyName, user.LastName),
		];

		JwtSecurityToken token = new(
			issuer: _configuration["Auth:ValidIssuer"],
			audience: _configuration["Auth:ValidAudience"],
			expires: DateTime.UtcNow.AddSeconds(expirationSeconds),
			claims: claims,
			signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);
		string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

		return new(accessToken, expirationSeconds, refreshToken);
	}

	// Generates a refresh token for the user and adds it to the DB
	private async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
	{
		// Generate a token
		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		int refreshTokenSize = _configuration.GetValue<int>("Auth:RefreshTokenSize");
		byte[] refreshTokenBytes = new byte[refreshTokenSize];
		rng.GetBytes(refreshTokenBytes);

		// Set the expiration date and assign token to the user
		int expirationDays = _configuration.GetValue<int>("Auth:RefreshTokenExpirationDays");
		UserRefreshToken tokenEntity = new()
		{
			Token = Convert.ToBase64String(refreshTokenBytes),
			UserId = user.Id,
			ValidTo = DateTime.UtcNow.AddDays(expirationDays)
		};

		// Save the token
		_dbContext.UserRefreshTokens.Add(tokenEntity);
		await _dbContext.SaveChangesAsync();

		// Return the token
		return tokenEntity.Token;
	}

	// Gets a user ID from the expired access token, returns null if token is invalid
	private async Task<string?> GetUserIdFromExpiredTokenAsync(string accessToken)
	{
		// Validate the access token
		string key = _configuration["Auth:SecretKey"]!;
		JwtSecurityTokenHandler tokenHandler = new();
		TokenValidationParameters validationParameters = new()
		{
			RequireExpirationTime = false, // Ignore expiration time
			ValidIssuer = _configuration["Auth:ValidIssuer"],
			ValidAudience = _configuration["Auth:ValidAudience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
		};
		var validationResult = await tokenHandler.ValidateTokenAsync(
			accessToken, validationParameters);
		// Return null if validation failed
		if (!validationResult.IsValid) return null;

		// Get ID of the user
		JwtSecurityToken jwtToken = (JwtSecurityToken)validationResult.SecurityToken;
		var subClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
		if (subClaim == null) return null;

		return subClaim.Value;
	}

	// Checks whether the refresh token is valid
	private async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
	{
		UserRefreshToken? tokenEntity = await _dbContext.UserRefreshTokens
			.Where(x => x.UserId == userId && x.Token == refreshToken)
			.FirstOrDefaultAsync();

		// Check if user owns the token and if it's still valid
		return tokenEntity != null && DateTime.UtcNow < tokenEntity.ValidTo;
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
