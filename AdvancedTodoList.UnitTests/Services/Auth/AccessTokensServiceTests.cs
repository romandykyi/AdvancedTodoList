using AdvancedTodoList.Application.Options;
using AdvancedTodoList.Application.Services.Implementations.Auth;
using AdvancedTodoList.Core.Models.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AdvancedTodoList.UnitTests.Services.Auth;

[TestFixture]
public class AccessTokensServiceTests
{
    private AccessTokenOptions _accessTokenOptions;
    private IOptions<AccessTokenOptions> _options;

    [SetUp]
    public void Setup()
    {
        _accessTokenOptions = new()
        {
            SecretKey = "TestSecretKeyWhichIsNotGoingToBeUsedInProductionOfCourse",
            ValidIssuer = "valid_issuer",
            ValidAudience = "valid_audience",
            ExpirationSeconds = 3600 // 1 hour expiration
        };
        _options = Substitute.For<IOptions<AccessTokenOptions>>();
        _options.Value.Returns(_accessTokenOptions);
    }

    [Test]
    public async Task GenerateAccessToken_ValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user_id",
            Email = "user@example.com",
            UserName = "username",
            FirstName = "John",
            LastName = "Doe"
        };
        var service = new AccessTokensService(_options);

        // Act
        string accessToken = service.GenerateAccessToken(user);

        // Assert
        TokenValidationParameters validationParameters = new()
        {
            ValidIssuer = _accessTokenOptions.ValidIssuer,
            ValidAudience = _accessTokenOptions.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenOptions.SecretKey))
        };
        JwtSecurityTokenHandler tokenHandler = new();
        var validationResult = await tokenHandler.ValidateTokenAsync(
            accessToken, validationParameters);
        // Assert that token is valid
        Assert.That(validationResult.IsValid);
        // Assert that token has all needed claims
        var token = (JwtSecurityToken)validationResult.SecurityToken;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value,
                Is.EqualTo(user.Id));
            Assert.That(token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value,
                Is.EqualTo(user.Email));
            Assert.That(token.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value,
                Is.EqualTo(user.UserName));
            Assert.That(token.Claims.First(x => x.Type == JwtRegisteredClaimNames.GivenName).Value,
                Is.EqualTo(user.FirstName));
            Assert.That(token.Claims.First(x => x.Type == JwtRegisteredClaimNames.FamilyName).Value,
                Is.EqualTo(user.LastName));
        }
    }

    [Test]
    public async Task GetUserIdFromExpiredTokenAsync_ValidToken_ReturnsUserId()
    {
        // Arrange
        // The access token is signed using '_options.SecretKey' key and has this payload:
        /*
		   {
				"sub": "test",
				"exp": 1577833200,
				"iss": "valid_issuer",
				"aud": "valid_audience"
			}
		 */
        // Token is expired in 01.01.2020
        var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0IiwiZXhwIjoxNTc3ODMzMjAwLCJpc3MiOiJ2YWxpZF9pc3N1ZXIiLCJhdWQiOiJ2YWxpZF9hdWRpZW5jZSJ9.ONo1hvc8wnKU3kcBZUr5xH--kdTLHXFAx2j3grC65CE";
        var service = new AccessTokensService(_options);

        // Act
        string? userId = await service.GetUserIdFromExpiredTokenAsync(accessToken);

        // Assert
        Assert.That(userId, Is.EqualTo("test"));
    }

    [Test]
    public async Task GetUserIdFromExpiredTokenAsync_InvalidToken_ReturnsNull()
    {
        // Arrange
        // The access token is signed using 'invalid' key and has this payload:
        /*
		   {
				"sub": "test",
				"exp": 1577833200,
				"iss": "valid_issuer",
				"aud": "valid_audience"
			}
		 */
        // Token is expired in 01.01.2020
        var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0IiwiZXhwIjoxNTc3ODMzMjAwLCJpc3MiOiJ2YWxpZF9pc3N1ZXIiLCJhdWQiOiJ2YWxpZF9hdWRpZW5jZSJ9.P_dE9VKK8KGMi5PMU9MW7_l0rrNrynxg4wThYxntXUs";
        var service = new AccessTokensService(_options);

        // Act
        string? userId = await service.GetUserIdFromExpiredTokenAsync(accessToken);

        // Assert
        Assert.That(userId, Is.Null);
    }
}
