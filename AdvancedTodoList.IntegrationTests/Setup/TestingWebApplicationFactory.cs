using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Testcontainers.MsSql;

namespace AdvancedTodoList.IntegrationTests.Setup;

public class TestingWebApplicationFactory(MsSqlContainer testDbContainer) : WebApplicationFactory<Program>
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
