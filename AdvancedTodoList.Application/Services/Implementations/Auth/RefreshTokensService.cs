using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Options;
using AdvancedTodoList.Core.Repositories;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace AdvancedTodoList.Application.Services.Implementations.Auth;

public class RefreshTokensService(
    IUserRefreshTokensRepository repository,
    IOptions<RefreshTokenOptions> options,
    IEntityExistenceChecker existenceChecker) : IRefreshTokensService
{
    private readonly IUserRefreshTokensRepository _repository = repository;
    private readonly RefreshTokenOptions _tokenOptions = options.Value;
    private readonly IEntityExistenceChecker _existenceChecker = existenceChecker;

    /// <summary>
    /// Generates a refresh token for the user and saves it asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user who will receive the token.</param>
    /// <returns>
    /// A string that represents a refresh token or <see langword="null" /> if user does not exist.
    /// </returns>
    public async Task<string?> GenerateAsync(string userId)
    {
        // Check if user exists
        if (!await _existenceChecker.ExistsAsync<ApplicationUser, string>(userId))
            return null;

        // Generate a token
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] refreshTokenBytes = new byte[_tokenOptions.Size];
        rng.GetBytes(refreshTokenBytes);

        // Set the expiration date and assign token to the user
        int expirationDays = _tokenOptions.ExpirationDays;
        UserRefreshToken tokenEntity = new()
        {
            Token = Convert.ToBase64String(refreshTokenBytes),
            UserId = userId,
            ValidTo = DateTime.UtcNow.AddDays(expirationDays)
        };

        // Save the token
        await _repository.AddAsync(tokenEntity);

        // Return the token value
        return tokenEntity.Token;
    }

    /// <summary>
    /// Revokes the refresh token of the user asynchronously
    /// </summary>
    /// <param name="userId">ID of the user whose token is being revoked.</param>
    /// <param name="token">Value of the token to be revoked.</param>
    /// <returns>
    /// <see langword="true" /> on success; <see langword="false" /> otherwise.
    /// </returns>
    public async Task<bool> RevokeAsync(string userId, string token)
    {
        var tokenEntity = await _repository.FindAsync(userId, token);
        if (tokenEntity == null) return false;

        // Delete the token
        await _repository.DeleteAsync(tokenEntity);

        return true;
    }

    /// <summary>
    /// Checks whether refresh token is valid asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user whose token is being validated.</param>
    /// <param name="token">Value of the token to be validated.</param>
    /// <returns>
    /// <see langword="true" /> if token is valid;
    /// <see langword="false" /> otherwise.
    /// </returns>
    public async Task<bool> ValidateAsync(string userId, string token)
    {
        var tokenEntity = await _repository.FindAsync(userId, token);
        return tokenEntity != null && DateTime.UtcNow < tokenEntity.ValidTo;
    }
}
