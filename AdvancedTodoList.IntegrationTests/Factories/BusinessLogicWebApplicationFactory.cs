using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace AdvancedTodoList.IntegrationTests.Factories;

/// <summary>
/// A web application factory used for testing business logic with mocked data access layer.
/// </summary>
public class BusinessLogicWebApplicationFactory : WebApplicationFactory<Program>
{
	public IEntityExistenceChecker EntityExistenceChecker { get; private set; } = null!;
	public IRepository<TodoList, string> TodoListsRepository { get; private set; } = null!;
	public IRepository<TodoItem, int> TodoItemsRepository { get; private set; } = null!;
	public IUserRefreshTokensRepository RefreshTokensRepository { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for the services
		EntityExistenceChecker = Substitute.For<IEntityExistenceChecker>();
		TodoListsRepository = Substitute.For<IRepository<TodoList, string>>();
		TodoItemsRepository = Substitute.For<IRepository<TodoItem, int>>();
		RefreshTokensRepository = Substitute.For<IUserRefreshTokensRepository>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => EntityExistenceChecker);
			services.AddScoped(_ => TodoListsRepository);
			services.AddScoped(_ => TodoItemsRepository);
			services.AddScoped(_ => RefreshTokensRepository);
		});
	}
}
