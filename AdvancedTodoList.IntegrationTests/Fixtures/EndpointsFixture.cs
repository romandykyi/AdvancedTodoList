using AdvancedTodoList.IntegrationTests.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdvancedTodoList.IntegrationTests.Fixtures;

/// <summary>
/// Abstract test fixture for endpoints.
/// </summary>
public abstract class EndpointsFixture
{
	protected IConfiguration Configuration { get; private set; }
	protected EndpointsWebApplicationFactory WebApplicationFactory { get; private set; }
	protected IServiceScope ServiceScope { get; private set; }

	public const string TestUserId = "TestUserId";

	protected HttpClient CreateAuthorizedHttpClient(string userId = TestUserId)
	{
		// Create a valid JWT
		string strKey = Configuration["Auth:SecretKey"]!;
		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(strKey));

		List<Claim> authClaims =
		[
			new(JwtRegisteredClaimNames.Sub, userId)
		];
		JwtSecurityToken token = new(
				issuer: Configuration["Auth:ValidIssuer"],
				audience: Configuration["Auth:ValidAudience"],
				expires: DateTime.UtcNow.AddMinutes(30),
				claims: authClaims,
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
				);

		string bearerToken = new JwtSecurityTokenHandler().WriteToken(token);

		var client = WebApplicationFactory.CreateClient();
		client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

		return client;
	}

	[SetUp]
	public void SetUpTest()
	{
		// Configure web application factory
		WebApplicationFactory = new EndpointsWebApplicationFactory();
		WebApplicationFactory.Server.PreserveExecutionContext = true;

		var scopeFactory = WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;
		ServiceScope = scopeFactory.CreateScope();
		Configuration = ServiceScope.ServiceProvider.GetService<IConfiguration>()!;
	}

	[TearDown]
	public Task TearDownTestAsync()
	{
		ServiceScope.Dispose();
		return WebApplicationFactory.DisposeAsync().AsTask();
	}
}
