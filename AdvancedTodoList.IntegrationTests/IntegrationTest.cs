﻿using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedTodoList.IntegrationTests;

/// <summary>
/// Base class for integration tests.
/// </summary>
public abstract class IntegrationTest
{
	private static bool s_migrated = false;

	protected static TestingWebApplicationFactory WebApplicationFactory { get; private set; }
	protected IServiceScopeFactory ScopeFactory { get; private set; }
	protected IServiceScope ServiceScope { get; private set; }
	protected ApplicationDbContext DbContext { get; private set; }

	[SetUp]
	public async Task SetUpServices()
	{
		// Configure web application factory
		WebApplicationFactory = new TestingWebApplicationFactory(IntegrationTestsSetup.TestDbContainer);
		WebApplicationFactory.Server.PreserveExecutionContext = true;

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

		await WebApplicationFactory.DisposeAsync();
	}
}
