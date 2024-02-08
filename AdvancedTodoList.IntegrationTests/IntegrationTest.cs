using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace AdvancedTodoList.IntegrationTests;

/// <summary>
/// Base class for integration tests.
/// </summary>
public abstract class IntegrationTest
{
	private static bool s_migrated = false;
	private static MsSqlContainer s_testDbContainer;
	protected static TestingWebApplicationFactory WebApplicationFactory { get; private set; }
	protected IServiceScopeFactory ScopeFactory { get; private set; }
	protected IServiceScope ServiceScope { get; private set; }
	protected ApplicationDbContext DbContext { get; private set; }

	[SetUp]
	public async Task SetUpServices()
	{
		// Get services needed for integration testing
		ScopeFactory = WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;
		ServiceScope = ScopeFactory.CreateScope();
		DbContext = ServiceScope.ServiceProvider.GetService<ApplicationDbContext>()!;
		// Migrate database if it isn't migrated yet
		if (!s_migrated)
		{
			await DbContext.Database.MigrateAsync();
			s_migrated = true;
		}
	}

	[TearDown]
	public async Task TearDownServicesAsync()
	{
		// Dispose resources
		await DbContext.DisposeAsync();
		ServiceScope.Dispose();
	}

	[OneTimeSetUp]
	public static async Task SetUpIntegrationTestAsync()
	{
		// Initialize and start a container with test DB
		s_testDbContainer = new MsSqlBuilder().Build();
		await s_testDbContainer.StartAsync();

		// Configure web application factory
		WebApplicationFactory = new TestingWebApplicationFactory(s_testDbContainer);
		WebApplicationFactory.Server.PreserveExecutionContext = true;
	}

	[OneTimeTearDown]
	public static async Task TearDownIntegrationTestAsync()
	{
		// Stop the DB container
		await s_testDbContainer.StopAsync();

		await WebApplicationFactory.DisposeAsync();
	}
}
