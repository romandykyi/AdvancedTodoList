using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace AdvancedTodoList.IntegrationTests.Factories;

/// <summary>
/// A web application factory used for testing endpoints with mocked business logic layer.
/// </summary>
public class EndpointsWebApplicationFactory : WebApplicationFactory<Program>
{
	public IAuthService AuthService { get; private set; } = null!;
	public ITodoListsService TodoListsService { get; private set; } = null!;
	public ITodoItemsService TodoItemsService { get; private set; } = null!;
	public IEntityExistenceChecker EntityExistenceChecker { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for the services
		AuthService = Substitute.For<IAuthService>();
		TodoListsService = Substitute.For<ITodoListsService>();
		TodoItemsService = Substitute.For<ITodoItemsService>();
		EntityExistenceChecker = Substitute.For<IEntityExistenceChecker>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => AuthService);
			services.AddScoped(_ => TodoListsService);
			services.AddScoped(_ => TodoItemsService);
			services.AddScoped(_ => EntityExistenceChecker);
		});
	}
}
