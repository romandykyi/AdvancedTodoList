using AdvancedTodoList.Infrastructure.Data;
using AdvancedTodoList.IntegrationTests.Factories;

namespace AdvancedTodoList.IntegrationTests.Fixtures;

/// <summary>
/// Abstract test fixture for data access layer.
/// </summary>
public abstract class DataAccessFixture
{
	private static bool s_migrated = false;

	protected static DataAccessWebApplicationFactory WebApplicationFactory { get; private set; }
	protected IServiceScope ServiceScope { get; private set; }
	protected ApplicationDbContext DbContext { get; private set; }

	[SetUp]
	public async Task SetUpServices()
	{
		// Configure web application factory
		WebApplicationFactory = new DataAccessWebApplicationFactory(TestContainersSetup.TestDbContainer);
		WebApplicationFactory.Server.PreserveExecutionContext = true;

		// Get services needed for integration testing
		var scopeFactory = WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;
		ServiceScope = scopeFactory.CreateScope();
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

		await WebApplicationFactory.DisposeAsync();
	}
}
