﻿using AdvancedTodoList.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedTodoList.RouteTests;

public class RouteTestsWebApplicationFactory : WebApplicationFactory<Program>
{
	public ITodoListsService TodoListsService { get; private set; } = null!;
	public ITodoItemsService TodoItemsService { get; private set; } = null!;
	public IEntityExistenceChecker EntityExistenceChecker { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Create mocks for the services
		TodoListsService = Substitute.For<ITodoListsService>();
		TodoItemsService = Substitute.For<ITodoItemsService>();
		EntityExistenceChecker = Substitute.For<IEntityExistenceChecker>();

		builder.ConfigureTestServices(services =>
		{
			services.AddScoped(_ => TodoListsService);
			services.AddScoped(_ => TodoItemsService);
			services.AddScoped(_ => EntityExistenceChecker);
		});
	}
}
