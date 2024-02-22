using Testcontainers.MsSql;

namespace AdvancedTodoList.IntegrationTests;

/// <summary>
/// Class that sets up integration testing environment.
/// </summary>
[SetUpFixture]
public static class TestContainersSetup
{
	/// <summary>
	/// Test container that contains a database.
	/// </summary>
	private static MsSqlContainer? s_testDbContainer;

	/// <summary>
	/// Gets a running test container or creates it if it's the first call.
	/// </summary>
	/// <returns>
	/// A running testing MS SQL container.
	/// </returns>
	public static async Task<MsSqlContainer> GetTestContainerAsync()
	{
		if (s_testDbContainer == null)
		{
			// Initialize and start a container with test DB
			s_testDbContainer = new MsSqlBuilder().Build();
			await s_testDbContainer.StartAsync();
		}
		return s_testDbContainer;
	}

	[OneTimeTearDown]
	public static async Task TearDown()
	{
		// Stop the DB container
		if (s_testDbContainer != null) await s_testDbContainer.StopAsync();
	}
}
