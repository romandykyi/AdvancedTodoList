using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

[TestFixture]
public class UserRefreshTokensRepositoryTests : BaseRepositoryTests<UserRefreshToken, int>
{
    private IUserRefreshTokensRepository TokensRepository => (IUserRefreshTokensRepository)Repository;

    protected override int NonExistingId => -1;

    private readonly DateTime UpdatedValidTo = DateTime.UtcNow.AddDays(180);

    protected override void UpdateEntity(UserRefreshToken entity)
    {
        entity.ValidTo = UpdatedValidTo;
    }

    protected override void AssertUpdated(UserRefreshToken updatedEntity)
    {
        Assert.That(updatedEntity.ValidTo, Is.EqualTo(UpdatedValidTo));
    }

    protected override async Task<UserRefreshToken> CreateTestEntityAsync()
    {
        var user = TestModels.CreateTestUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();

        return TestModels.CreateTestUserRefreshToken(user.Id);
    }

    [Test]
    public async Task FindAsync_TokenExists_ReturnsValidToken()
    {
        // Arrange
        var user = TestModels.CreateTestUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();
        var token = TestModels.CreateTestUserRefreshToken(user.Id);
        DbContext.Add(token);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await TokensRepository.FindAsync(user.Id, token.Token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(token.Id));
    }

    [Test]
    public async Task FindAsync_TokenDoesNotExist_ReturnsNull()
    {
        // Arrange
        string userId = "user_id";
        string token = "no token :(";

        // Act
        var result = await TokensRepository.FindAsync(userId, token);

        // Assert
        Assert.That(result, Is.Null);
    }
}
