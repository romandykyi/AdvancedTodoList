using Testcontainers.MsSql;

namespace AdvancedTodoList.IntegrationTests;

/// <summary>
/// Class that sets up integration testing environment.
/// </summary>
[SetUpFixture]
public static class IntegrationTestsSetup
{
    /// <summary>
    /// Test container that contains a database.
    /// </summary>
    public static MsSqlContainer TestDbContainer { get; private set; }

    [OneTimeSetUp]
    public static async Task SetUpIntegrationTests()
    {
        // Initialize and start a container with test DB
        TestDbContainer = new MsSqlBuilder().Build();
        await TestDbContainer.StartAsync();
    }

    [OneTimeTearDown]
    public static async Task TearDownIntegrationTests()
    {
        // Stop the DB container
        await TestDbContainer.StopAsync();
    }
}
