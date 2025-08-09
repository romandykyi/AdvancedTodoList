using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using AdvancedTodoList.Application.Services.Definitions.Auth;

namespace AdvancedTodoList.IntegrationTests.Services.Auth;

[TestFixture]
public class RefreshTokensServiceTests : BusinessLogicFixture
{
	private IRefreshTokensService _service;

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<IRefreshTokensService>()!;
	}

	[Test]
	public async Task GenerateAsync_UserExists_ReturnsValidToken()
	{
		// Arrange
		string userId = "userId";
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<ApplicationUser, string>(userId)
			.Returns(true);
		WebApplicationFactory.RefreshTokensRepository
			.AddAsync(Arg.Any<UserRefreshToken>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.GenerateAsync(userId);

		// Assert
		Assert.That(result, Is.Not.Null);
		// Refresh token is not a default base64 string
		byte[] base64Token = Convert.FromBase64String(result);
		Assert.That(base64Token.All(x => x == 0), Is.False);
		// Refresh token was created
		await WebApplicationFactory.RefreshTokensRepository
			.Received()
			.AddAsync(Arg.Is<UserRefreshToken>(x => x.Token == result && x.UserId == userId));
	}

	[Test]
	public async Task GenerateAsync_UserDoesNotExist_ReturnsNull()
	{
		// Arrange
		string userId = "userId";
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<ApplicationUser, string>(userId)
			.Returns(false);

		// Act
		var result = await _service.GenerateAsync(userId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task RevokeAsync_TokenExists_ReturnsTrueAndDeletesToken()
	{
		// Arrange
		string userId = "userId";
		UserRefreshToken refreshToken = TestModels.CreateTestUserRefreshToken(userId);
		WebApplicationFactory.RefreshTokensRepository
			.FindAsync(userId, refreshToken.Token)
			.Returns(refreshToken);
		WebApplicationFactory.RefreshTokensRepository
			.DeleteAsync(Arg.Any<UserRefreshToken>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.RevokeAsync(userId, refreshToken.Token);

		// Assert
		Assert.That(result, Is.True);
		// Refresh token was deleted
		await WebApplicationFactory.RefreshTokensRepository
			.Received()
			.DeleteAsync(Arg.Is<UserRefreshToken>(x => x.Token == refreshToken.Token && x.UserId == userId));
	}

	[Test]
	public async Task RevokeAsync_TokenDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string userId = "userId";
		UserRefreshToken refreshToken = TestModels.CreateTestUserRefreshToken(userId);
		WebApplicationFactory.RefreshTokensRepository
			.FindAsync(userId, refreshToken.Token)
			.ReturnsNull();

		// Act
		var result = await _service.RevokeAsync(userId, refreshToken.Token);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task ValidateAsync_ValidToken_ReturnsTrue()
	{
		// Arrange
		string userId = "userId";
		UserRefreshToken refreshToken = TestModels.CreateTestUserRefreshToken(userId);
		WebApplicationFactory.RefreshTokensRepository
			.FindAsync(userId, refreshToken.Token)
			.Returns(refreshToken);

		// Act
		var result = await _service.ValidateAsync(userId, refreshToken.Token);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task ValidateAsync_TokenDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string userId = "userId";
		UserRefreshToken refreshToken = TestModels.CreateTestUserRefreshToken(userId);
		WebApplicationFactory.RefreshTokensRepository
			.FindAsync(userId, refreshToken.Token)
			.ReturnsNull();

		// Act
		var result = await _service.ValidateAsync(userId, refreshToken.Token);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task ValidateAsync_OutdatedToken_ReturnsFalse()
	{
		// Arrange
		string userId = "userId";
		UserRefreshToken refreshToken = TestModels.CreateTestUserRefreshToken(userId);
		refreshToken.ValidTo = DateTime.UtcNow.AddDays(-30);
		WebApplicationFactory.RefreshTokensRepository
			.FindAsync(userId, refreshToken.Token)
			.Returns(refreshToken);

		// Act
		var result = await _service.ValidateAsync(userId, refreshToken.Token);

		// Assert
		Assert.That(result, Is.False);
	}
}
