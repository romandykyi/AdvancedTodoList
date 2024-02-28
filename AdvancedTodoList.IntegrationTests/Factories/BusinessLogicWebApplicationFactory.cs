using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.IntegrationTests.Services;
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
	public IRepository<TestTodoListDependantEntity, int> TestTodoListDependantEntitiesRepository { get; private set; } = null!;
	public ITodoListDependantEntitiesService<TodoItem, int> TodoItemsHelperService { get; set; } = null!;
	public ITodoListDependantEntitiesService<TodoListRole, int> TodoRolesHelperService { get; set; } = null!;
	public ITodoListDependantEntitiesService<TodoListMember, int> TodoMembersHelperService { get; set; } = null!;
	public IUserRefreshTokensRepository RefreshTokensRepository { get; private set; } = null!;
	public ITodoListMembersRepository TodoListMembersRepository { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for the services
		EntityExistenceChecker = Substitute.For<IEntityExistenceChecker>();
		TodoListsRepository = Substitute.For<IRepository<TodoList, string>>();
		TodoItemsRepository = Substitute.For<IRepository<TodoItem, int>>();
		TestTodoListDependantEntitiesRepository = Substitute.For<IRepository<TestTodoListDependantEntity, int>>();
		TodoItemsHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoItem, int>>();
		TodoRolesHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoListRole, int>>();
		TodoMembersHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoListMember, int>>();
		RefreshTokensRepository = Substitute.For<IUserRefreshTokensRepository>();
		TodoListMembersRepository = Substitute.For<ITodoListMembersRepository>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => EntityExistenceChecker);
			services.AddScoped(_ => TodoListsRepository);
			services.AddScoped(_ => TodoItemsRepository);
			services.AddScoped(_ => TestTodoListDependantEntitiesRepository);
			services.AddScoped(_ => TodoItemsHelperService);
			services.AddScoped(_ => TodoRolesHelperService);
			services.AddScoped(_ => TodoMembersHelperService);
			services.AddScoped(_ => RefreshTokensRepository);
			services.AddScoped(_ => TodoListMembersRepository);
		});
	}
}
