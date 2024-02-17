using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdvancedTodoList.RouteTests;

/// <summary>
/// Base class for route tests.
/// </summary>
public abstract class RouteTest
{
	protected IConfiguration Configuration { get; private set; }
	protected RouteTestsWebApplicationFactory WebApplicationFactory { get; private set; }
	protected IServiceScopeFactory ScopeFactory { get; private set; }
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
		WebApplicationFactory = new RouteTestsWebApplicationFactory();
		WebApplicationFactory.Server.PreserveExecutionContext = true;

		ScopeFactory = WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;
		ServiceScope = ScopeFactory.CreateScope();
		Configuration = ServiceScope.ServiceProvider.GetService<IConfiguration>()!;
	}

	[TearDown]
	public Task TearDownTestAsync()
	{
		ServiceScope.Dispose();
		return WebApplicationFactory.DisposeAsync().AsTask();
	}
}
