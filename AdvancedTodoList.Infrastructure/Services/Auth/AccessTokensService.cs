using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Options;
using AdvancedTodoList.Core.Services.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdvancedTodoList.Infrastructure.Services.Auth;

/// <summary>
/// A service that manages access tokens.
/// </summary>
public class AccessTokensService(AccessTokenOptions options) : IAccessTokensService
{
	private readonly AccessTokenOptions _options = options;

	/// <summary>
	/// Generates an access token for the user.
	/// </summary>
	/// <param name="user">User which will receive an access token.</param>
	/// <returns>
	/// A string that represents an access token.
	/// </returns>
	public string GenerateAccessToken(ApplicationUser user)
	{
		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_options.SecretKey));

		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(JwtRegisteredClaimNames.Email, user.Email!),
			new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
			new(JwtRegisteredClaimNames.GivenName, user.FirstName),
			new(JwtRegisteredClaimNames.FamilyName, user.LastName),
		];

		JwtSecurityToken token = new(
			issuer: _options.ValidIssuer,
			audience: _options.ValidAudience,
			expires: DateTime.UtcNow.AddSeconds(_options.ExpirationSeconds),
			claims: claims,
			signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);
		string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

		return accessToken;
	}

	/// <summary>
	/// Validates an access token without checking expiration time and then returns 
	/// ID of the user stored in it asynchronously.
	/// </summary>
	/// <param name="accessToken">A string that represents an access token.</param>
	/// <returns>
	/// A user ID retrieved from the access token or <see langword="null" />, if validation failed.
	/// </returns>
	public async Task<string?> GetUserIdFromExpiredTokenAsync(string accessToken)
	{
		// Validate the access token
		string key = _options.SecretKey;
		JwtSecurityTokenHandler tokenHandler = new();
		TokenValidationParameters validationParameters = new()
		{
			ValidateLifetime = false, // Ignore expiration time
			ValidIssuer = _options.ValidIssuer,
			ValidAudience = _options.ValidAudience,
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
}
