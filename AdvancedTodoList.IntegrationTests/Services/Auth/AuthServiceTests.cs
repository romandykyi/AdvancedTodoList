using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdvancedTodoList.IntegrationTests.Services.Auth;

[TestFixture]
public class AuthServiceTests : DataAccessFixture
{
    private UserManager<ApplicationUser> _userManager;
    private IAuthService _authService;
    private IConfiguration _configuration;
    private const string TestPassword = "VeWyStW0nGPa$$w0rD";

    // Creates a test user and adds it to the DB
    private async Task<ApplicationUser> CreateTestUserAsync()
    {
        ApplicationUser user = TestModels.CreateTestUser();

        var result = await _userManager.CreateAsync(user, TestPassword);
        Assert.That(result.Succeeded, "Failed to create a test user");

        return user;
    }

    // Creates a test refresh token for the user and adds it to the DB
    private async Task<UserRefreshToken> CreateTestRefreshTokenAsync(ApplicationUser user, bool expired = false)
    {
        UserRefreshToken token = new()
        {
            UserId = user.Id,
            ValidTo = expired ? DateTime.UtcNow - TimeSpan.FromDays(366) : DateTime.UtcNow + TimeSpan.FromDays(366),
            Token = Guid.NewGuid().ToString().Replace('-', 'A')
        };
        DbContext.UserRefreshTokens.Add(token);
        await DbContext.SaveChangesAsync();

        return token;
    }

    // Creates a test JWT for a user
    private string CreateTestJwt(ApplicationUser user, bool valid = true, DateTime? expires = null)
    {
        string strKey = valid ? _configuration["Auth:AccessToken:SecretKey"]! : "Invalid" + _configuration["Auth:AccessToken:SecretKey"]!;
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(strKey));

        List<Claim> authClaims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id)
        ];
        JwtSecurityToken token = new(
                issuer: _configuration["Auth:AccessToken:ValidIssuer"],
                audience: _configuration["Auth:AccessToken:ValidAudience"],
                expires: expires ?? DateTime.UtcNow.AddMinutes(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [SetUp]
    public void SetUp()
    {
        _userManager = ServiceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>()!;
        _authService = ServiceScope.ServiceProvider.GetService<IAuthService>()!;
        _configuration = ServiceScope.ServiceProvider.GetService<IConfiguration>()!;
    }

    [TearDown]
    public void TearDown()
    {
        _userManager.Dispose();
    }

    // Assert that LogInResponse is valid
    private async Task AssertSuccessLogInAsync(LogInResponse? response, ApplicationUser user)
    {
        // Assert that response is not null (success)
        Assert.That(response, Is.Not.Null);
        // Assert that access token is provided
        Assert.That(response.AccessToken, Is.Not.Null);
        // Assert that access token is valid
        JwtSecurityTokenHandler tokenHandler = new();
        string key = _configuration["Auth:AccessToken:SecretKey"]!;
        TokenValidationParameters validationParameters = new()
        {
            ValidIssuer = _configuration["Auth:AccessToken:ValidIssuer"],
            ValidAudience = _configuration["Auth:AccessToken:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
        tokenHandler.ValidateToken(response.AccessToken, validationParameters, out var token);
        var jwtToken = (JwtSecurityToken)token;
        var claims = jwtToken.Claims;

        // Assert that claims are correct
        using (Assert.EnterMultipleScope())
        {
            Assert.That(claims.Single(x => x.Type == JwtRegisteredClaimNames.Sub).Value, Is.EqualTo(user.Id));
            Assert.That(claims.Single(x => x.Type == JwtRegisteredClaimNames.GivenName).Value, Is.EqualTo(user.FirstName));
            Assert.That(claims.Single(x => x.Type == JwtRegisteredClaimNames.FamilyName).Value, Is.EqualTo(user.LastName));
            Assert.That(claims.Single(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value, Is.EqualTo(user.UserName));
            Assert.That(claims.Single(x => x.Type == JwtRegisteredClaimNames.Email).Value, Is.EqualTo(user.Email));
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable NUnit2045 // Use Assert.Multiple

        // Assert that refresh token is present
        Assert.That(
            await DbContext.UserRefreshTokens
            .AnyAsync(x => x.Token == response.RefreshToken && x.UserId == user.Id)
            );
        // Assert that refresh token is not a default base 64 string
        Assert.That(Convert.FromBase64String(response.RefreshToken).All(x => x == 0), Is.False);

#pragma warning restore NUnit2045 // Use Assert.Multiple
#pragma warning restore IDE0079 // Remove unnecessary suppression
    }

    [Test]
    public async Task LogInAsync_UserNameUsed_LogsIn()
    {
        // Arrange
        ApplicationUser user = await CreateTestUserAsync();
        LogInDto dto = new(user.UserName!, TestPassword);

        // Act
        var response = await _authService.LogInAsync(dto);

        // Assert
        await AssertSuccessLogInAsync(response, user);
    }

    [Test]
    public async Task LogInAsync_EmailUsed_LogsIn()
    {
        // Arrange
        ApplicationUser user = await CreateTestUserAsync();
        LogInDto dto = new(user.Email!, TestPassword);

        // Act
        var response = await _authService.LogInAsync(dto);

        // Assert
        await AssertSuccessLogInAsync(response, user);
    }

    [Test]
    public async Task LogInAsync_InvalidCredentials_Fails()
    {
        // Arrange
        ApplicationUser user = await CreateTestUserAsync();
        LogInDto dto = new(user.Email!, "wrong");

        // Act
        var response = await _authService.LogInAsync(dto);

        // Assert failure
        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task RegisterAsync_ValidData_CreatesUser()
    {
        // Arrange
        RegisterDto dto = new("uniq@example.com", "Username", "First", "Last", TestPassword);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert that result indicates success
        Assert.That(result.IsSuccess);
        // Assert that user is created
        var user = await DbContext.Users.Where(
            x => x.Email == dto.Email && x.UserName == dto.UserName &&
            x.FirstName == dto.FirstName && x.LastName == dto.LastName
            )
            .SingleOrDefaultAsync();
        Assert.That(user, Is.Not.Null);
        // Assert that password is correct
        var passwordResult = await _userManager.CheckPasswordAsync(user, dto.Password);
        Assert.That(passwordResult);
    }

    [Test]
    public async Task RegisterAsync_DuplicateEmail_Fails()
    {
        // Arrange
        var testUser = await CreateTestUserAsync();
        RegisterDto dto = new(testUser.Email!, "Username", "First", "Last", TestPassword);

        // Act
        var result = await _authService.RegisterAsync(dto);
        using (Assert.EnterMultipleScope())
        {
            // Assert that result does not indicate success
            Assert.That(result.IsSuccess, Is.False);
            // Assert that 'Email' property caused this error
            Assert.That(result.Errors.Any(x => x.Property == "Email"), "RegisterResult does not indicate that 'Email' property caused the error.");
        }
    }

    [Test]
    public async Task RegisterAsync_DuplicateUserName_Fails()
    {
        // Arrange
        var testUser = await CreateTestUserAsync();
        RegisterDto dto = new("uniqe@example.com", testUser.UserName!, "First", "Last", TestPassword);

        // Act
        var result = await _authService.RegisterAsync(dto);
        using (Assert.EnterMultipleScope())
        {
            // Assert that result does not indicate success
            Assert.That(result.IsSuccess, Is.False);
            // Assert that 'UserName' property caused this error
            Assert.That(result.Errors.Any(x => x.Property == "UserName"), "RegisterResult does not indicate that 'UserName' property caused the error.");
        }
    }

    [Test]
    public async Task RefreshAsync_ValidTokens_Succeeds()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        UserRefreshToken token = await CreateTestRefreshTokenAsync(user);
        string expiredToken = CreateTestJwt(user, expires: DateTime.UtcNow.AddDays(-5));
        RefreshDto dto = new(expiredToken, token.Token);

        // Act
        var response = await _authService.RefreshAsync(dto);

        // Assert
        await AssertSuccessLogInAsync(response, user);
    }

    [Test]
    public async Task RefreshAsync_RefreshTokenOfAnotherUser_Fails()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var user2 = await CreateTestUserAsync();
        UserRefreshToken token = await CreateTestRefreshTokenAsync(user2);
        RefreshDto dto = new(CreateTestJwt(user), token.Token);

        // Act
        var response = await _authService.RefreshAsync(dto);

        // Assert
        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task RefreshAsync_InvalidRefreshToken_Fails()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        RefreshDto dto = new(CreateTestJwt(user), "Token which does not exist");

        // Act
        var response = await _authService.RefreshAsync(dto);

        // Assert
        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task RefreshAsync_ExpiredRefreshToken_Fails()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        UserRefreshToken token = await CreateTestRefreshTokenAsync(user, expired: true);
        RefreshDto dto = new(CreateTestJwt(user), token.Token);

        // Act
        var response = await _authService.RefreshAsync(dto);

        // Assert
        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task RefreshAsync_InvalidAccessToken_Fails()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        UserRefreshToken token = new()
        {
            UserId = user.Id,
            ValidTo = DateTime.UtcNow + TimeSpan.FromDays(366),
            Token = Guid.NewGuid().ToString()
        };
        RefreshDto dto = new(CreateTestJwt(user, valid: false), token.Token);

        // Act
        var response = await _authService.RefreshAsync(dto);

        // Assert
        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task LogOutAsync_ValidInput_ReturnsTrueAndRevokesToken()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var refreshToken = await CreateTestRefreshTokenAsync(user);
        LogOutDto dto = new(refreshToken.Token);

        // Act
        var result = await _authService.LogOutAsync(user.Id, dto);

        // Assert
        Assert.That(result);
    }

    [Test]
    public async Task LogOutAsync_InvalidUserId_ReturnsFalse()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var refreshToken = await CreateTestRefreshTokenAsync(user);
        LogOutDto dto = new(refreshToken.Token);

        // Act
        var result = await _authService.LogOutAsync("InvalidId", dto);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task LogOutAsync_InvalidRefreshToken_ReturnsFalse()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        LogOutDto dto = new("Invalid refresh token");

        // Act
        var result = await _authService.LogOutAsync(user.Id, dto);

        // Assert
        Assert.That(result, Is.False);
    }
}
