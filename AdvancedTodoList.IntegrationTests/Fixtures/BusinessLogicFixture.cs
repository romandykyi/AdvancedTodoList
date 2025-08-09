using AdvancedTodoList.IntegrationTests.Factories;

namespace AdvancedTodoList.IntegrationTests.Fixtures;

/// <summary>
/// Abstract test fixture for business layer.
/// </summary>
public abstract class BusinessLogicFixture
{
    protected BusinessLogicWebApplicationFactory WebApplicationFactory { get; private set; }
    protected IServiceScope ServiceScope { get; private set; }

    [SetUp]
    public void SetUpTest()
    {
        // Configure web application factory
        WebApplicationFactory = new BusinessLogicWebApplicationFactory();
        WebApplicationFactory.Server.PreserveExecutionContext = true;

        var scopeFactory = WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;
        ServiceScope = scopeFactory.CreateScope();
    }

    [TearDown]
    public async Task TearDownTestAsync()
    {
        ServiceScope.Dispose();
        await WebApplicationFactory.DisposeAsync();
    }
}
