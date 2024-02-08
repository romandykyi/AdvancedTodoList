using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Data.Common;

namespace AdvancedTodoList.RouteTests;

public class RouteTestsWebApplicationFactory : WebApplicationFactory<Program>
{
	public ITodoListsService TodoListsService { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for services
		TodoListsService = Substitute.For<ITodoListsService>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => TodoListsService);
		});
	}
}
