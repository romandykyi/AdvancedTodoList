using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace AdvancedTodoList.IntegrationTests.Factories;

/// <summary>
/// A web application factory used for testing business logic with mocked data access layer.
/// </summary>
public class BusinessLogicWebApplicationFactory : WebApplicationFactory<Program>
{
	public IRepository<TodoList, string> TodoListsRepository { get; private set; } = null!;
	public IRepository<TodoItem, int> TodoItemsRepository { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for the services
		TodoListsRepository = Substitute.For<IRepository<TodoList, string>>();
		TodoItemsRepository = Substitute.For<IRepository<TodoItem, int>>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => TodoListsRepository);
			services.AddScoped(_ => TodoItemsRepository);
		});
	}
}
