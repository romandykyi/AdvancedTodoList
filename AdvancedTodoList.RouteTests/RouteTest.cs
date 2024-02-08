using Microsoft.Extensions.DependencyInjection;

namespace AdvancedTodoList.RouteTests;

/// <summary>
/// Base class for route tests.
/// </summary>
public abstract class RouteTest
{
	protected RouteTestsWebApplicationFactory WebApplicationFactory { get; private set; }
	protected IServiceScopeFactory ScopeFactory { get; private set; }
	protected IServiceScope ServiceScope { get; private set; }

	[SetUp]
	public void SetUpTest()
	{
		// Configure web application factory
		WebApplicationFactory = new RouteTestsWebApplicationFactory();
		WebApplicationFactory.Server.PreserveExecutionContext = true;

		ScopeFactory = WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;
		ServiceScope = ScopeFactory.CreateScope();
	}

	[TearDown]
	public Task TearDownTestAsync()
	{
		ServiceScope.Dispose();
		return WebApplicationFactory.DisposeAsync().AsTask();
	}
}
