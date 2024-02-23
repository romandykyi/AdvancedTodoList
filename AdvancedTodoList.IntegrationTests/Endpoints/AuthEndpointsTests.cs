using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.IntegrationTests.Fixtures;
using NSubstitute.ReturnsExtensions;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class AuthEndpointsTests : EndpointsFixture
{
	[Test]
	public async Task LogIn_ValidCredentials_SucceedsAndReturnsValidResponse()
	{
		// Arrange
		LogInDto logInDto = new("UserName", "Password");
		LogInResponse expectedResponse = new("Access", "Refresh");
		using var client = WebApplicationFactory.CreateClient();

		WebApplicationFactory.AuthService
			.LogInAsync(logInDto)
			.Returns(expectedResponse);

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logIn", logInDto);
		var body = await result.Content.ReadFromJsonAsync<LogInResponse>();

		// Assert success status code
		result.EnsureSuccessStatusCode();
		// Assert that response is valid
		Assert.That(body, Is.EqualTo(expectedResponse));
	}

	[Test]
	public async Task LogIn_InvalidDto_Returns400()
	{
		// Arrange
		LogInDto logInDto = new("", "");
		using var client = WebApplicationFactory.CreateClient();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logIn", logInDto);

		// Arrange
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task LogIn_InvalidCredentials_Returns401()
	{
		// Arrange
		LogInDto logInDto = new("UserName", "FakePassword");
		using var client = WebApplicationFactory.CreateClient();

		WebApplicationFactory.AuthService
			.LogInAsync(logInDto)
			.ReturnsNull();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logIn", logInDto);

		// Arrange
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task Register_ValidRegisterAttempt_Succeeds()
	{
		// Arrange
		RegisterDto registerDto = new("email@example.com", "UserName", "1st", "last", "M@ster_Pa$5w0r4");
		using var client = WebApplicationFactory.CreateClient();

		WebApplicationFactory.AuthService
			.RegisterAsync(registerDto)
			.Returns(RegisterResult.Success());

		// Act
		var result = await client.PostAsJsonAsync("api/auth/register", registerDto);

		// Arrange
		result.EnsureSuccessStatusCode();
	}

	[Test]
	public async Task Register_InvalidRegisterAttempt_Returns400()
	{
		// Arrange
		RegisterDto registerDto = new("email@example.com", "UserName", "1st", "last", "M@ster_Pa$5w0r4");
		using var client = WebApplicationFactory.CreateClient();

		WebApplicationFactory.AuthService
			.RegisterAsync(registerDto)
			.Returns(RegisterResult.Failure([new("$", "error")]));

		// Act
		var result = await client.PostAsJsonAsync("api/auth/register", registerDto);

		// Arrange
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task Register_InvalidRegisterDto_Returns400()
	{
		// Arrange
		RegisterDto registerDto = new("invalid email", "~$#", "", "", "weakpassword");
		using var client = WebApplicationFactory.CreateClient();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/register", registerDto);

		// Arrange
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task Refresh_ValidTokens_SucceedsAndReturnsValidResponse()
	{
		// Arrange
		RefreshDto refreshDto = new("Access", "Refresh");
		LogInResponse expectedResponse = new("Access", "Refresh");
		using var client = WebApplicationFactory.CreateClient();

		WebApplicationFactory.AuthService
			.RefreshAsync(refreshDto)
			.Returns(expectedResponse);

		// Act
		var result = await client.PostAsJsonAsync("api/auth/refresh", refreshDto);
		var body = await result.Content.ReadFromJsonAsync<LogInResponse>();

		// Assert success status code
		result.EnsureSuccessStatusCode();
		// Assert that response is valid
		Assert.That(body, Is.EqualTo(expectedResponse));
	}

	[Test]
	public async Task Refresh_InvalidRefreshDto_Returns400()
	{
		// Arrange
		RefreshDto refreshDto = new("", " ");
		using var client = WebApplicationFactory.CreateClient();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/refresh", refreshDto);

		// Arrange
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task Refresh_InvalidTokens_Returns401()
	{
		// Arrange
		RefreshDto refreshDto = new("InvalidAccess", "InvalidRefresh");
		using var client = WebApplicationFactory.CreateClient();

		WebApplicationFactory.AuthService
			.RefreshAsync(refreshDto)
			.ReturnsNull();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/refresh", refreshDto);

		// Arrange
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task LogOut_ValidTokenProvided_Succeeds()
	{
		// Arrange
		LogOutDto dto = new("Refresh");
		string userId = "UserId";
		using var client = CreateAuthorizedHttpClient(userId);

		WebApplicationFactory.AuthService
			.LogOutAsync(userId, dto)
			.Returns(true);

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logOut", dto);

		// Assert
		result.EnsureSuccessStatusCode();
	}

	[Test]
	public async Task LogOut_InvalidTokenProvided_Returns401()
	{
		// Arrange
		LogOutDto dto = new("Invalid");
		string userId = "UserId";
		using var client = CreateAuthorizedHttpClient(userId);

		WebApplicationFactory.AuthService
			.LogOutAsync(userId, dto)
			.Returns(false);

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logOut", dto);

		// Assert
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task LogOut_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		LogOutDto dto = new("Refresh");
		using var client = WebApplicationFactory.CreateClient();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logOut", dto);

		// Assert
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task LogOut_InvalidDto_Returns400()
	{
		// Arrange
		LogOutDto dto = new("");
		using var client = CreateAuthorizedHttpClient();

		// Act
		var result = await client.PostAsJsonAsync("api/auth/logOut", dto);

		// Assert
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}
}
