using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Repositories;
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
	public ITodoItemCategoriesService TodoItemCategoriesService { get; private set; } = null!;
	public ITodoListRolesService TodoListRolesService { get; private set; } = null!;
	public ITodoListMembersService TodoListMembersService { get; private set; } = null!;
	public IInvitationLinksService InvitationLinksService { get; private set; } = null!;
	public IEntityExistenceChecker EntityExistenceChecker { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for the services
		AuthService = Substitute.For<IAuthService>();
		TodoListsService = Substitute.For<ITodoListsService>();
		TodoItemsService = Substitute.For<ITodoItemsService>();
		TodoItemCategoriesService = Substitute.For<ITodoItemCategoriesService>();
		TodoListRolesService = Substitute.For<ITodoListRolesService>();
		TodoListMembersService = Substitute.For<ITodoListMembersService>();
		InvitationLinksService = Substitute.For<IInvitationLinksService>();
		EntityExistenceChecker = Substitute.For<IEntityExistenceChecker>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => AuthService);
			services.AddScoped(_ => TodoListsService);
			services.AddScoped(_ => TodoItemsService);
			services.AddScoped(_ => TodoItemCategoriesService);
			services.AddScoped(_ => TodoListRolesService);
			services.AddScoped(_ => TodoListMembersService);
			services.AddScoped(_ => InvitationLinksService);
			services.AddScoped(_ => EntityExistenceChecker);
		});
	}
}
