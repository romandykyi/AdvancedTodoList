using AdvancedTodoList.Core.Models.Auth;

namespace AdvancedTodoList.Core.Repositories;

/// <summary>
/// Represents a repository interface for CRUD operations on user refresh tokens.
/// </summary>
public interface IUserRefreshTokensRepository : IRepository<UserRefreshToken, int>
{
	/// <summary>
	/// Finds user's refresh token by user's ID and refresh token's value asynchronously.
	/// </summary>
	/// <param name="userId">User's unique identifier</param>
	/// <param name="refreshToken">Value of the refresh token.</param>
	/// <returns>
	/// Found user's refresh token, or <see langword="null" /> if it was not found.
	/// </returns>
	Task<UserRefreshToken?> FindAsync(string userId, string refreshToken);
}
