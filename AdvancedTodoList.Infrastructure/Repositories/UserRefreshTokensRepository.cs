using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for perfoming CRUD operations on users refresh tokens.
/// </summary>
public class UserRefreshTokensRepository(ApplicationDbContext dbContext) :
	BaseRepository<UserRefreshToken, int>(dbContext), IUserRefreshTokensRepository
{
	/// <summary>
	/// Finds user's refresh token by user's ID and refresh token's value asynchronously.
	/// </summary>
	/// <param name="userId">User's unique identifier</param>
	/// <param name="refreshToken">Value of the refresh token.</param>
	/// <returns>
	/// Found user's refresh token, or <see langword="null" /> if it was not found.
	/// </returns>
	public async Task<UserRefreshToken?> FindAsync(string userId, string refreshToken)
	{
		return await DbContext.UserRefreshTokens
			.Where(x => x.UserId == userId && x.Token == refreshToken)
			.FirstOrDefaultAsync();
	}
}
