using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Data.Common;
using Testcontainers.MsSql;

namespace AdvancedTodoList.IntegrationTests.Factories;

/// <summary>
/// A web application factory used for integration testing data access layer on a test container DB.
/// </summary>
public class DataAccessWebApplicationFactory(MsSqlContainer testDbContainer) : WebApplicationFactory<Program>
{
	private readonly MsSqlContainer _testDbContainer = testDbContainer;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			var dbContextOptions = services.SingleOrDefault(service => typeof(DbContextOptions<ApplicationDbContext>) == service.ServiceType);
			if (dbContextOptions != null)
			{
				services.Remove(dbContextOptions);
			}
			var dbConnection = services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType);
			if (dbConnection != null)
			{
				services.Remove(dbConnection);
			}
			services.AddDbContext<ApplicationDbContext>((_, option) => option.UseSqlServer(_testDbContainer.GetConnectionString()));
		});
	}
}
