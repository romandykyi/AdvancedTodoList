using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
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
    public IRepository<TodoItemCategory, int> TodoItemCategoriesRepository { get; private set; } = null!;
    public IRepository<TestTodoListDependantEntity, int> TestTodoListDependantEntitiesRepository { get; private set; } = null!;
    public IInvitationLinksRepository InvitationLinksRepository { get; private set; } = null!;
    public ITodoListDependantEntitiesService<TodoItem, int> TodoItemsHelperService { get; set; } = null!;
    public ITodoListDependantEntitiesService<TodoItemCategory, int> TodoItemCategoriesHelperService { get; set; } = null!;
    public ITodoListDependantEntitiesService<TodoListRole, int> TodoRolesHelperService { get; set; } = null!;
    public ITodoListDependantEntitiesService<TodoListMember, int> TodoMembersHelperService { get; set; } = null!;
    public IUserRefreshTokensRepository RefreshTokensRepository { get; private set; } = null!;
    public IPermissionsChecker PermissionsChecker { get; private set; } = null!;
    public IRepository<TodoListRole, int> TodoListRolesRepository { get; private set; } = null!;
    public ITodoListMembersRepository TodoListMembersRepository { get; private set; } = null!;
    public IUnitOfWork UnitOfWork { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Create mocks for the services
        EntityExistenceChecker = Substitute.For<IEntityExistenceChecker>();
        TodoListsRepository = Substitute.For<IRepository<TodoList, string>>();
        TodoItemsRepository = Substitute.For<IRepository<TodoItem, int>>();
        TodoItemCategoriesRepository = Substitute.For<IRepository<TodoItemCategory, int>>();
        TestTodoListDependantEntitiesRepository = Substitute.For<IRepository<TestTodoListDependantEntity, int>>();
        InvitationLinksRepository = Substitute.For<IInvitationLinksRepository>();
        TodoItemsHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoItem, int>>();
        TodoItemCategoriesHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoItemCategory, int>>();
        TodoRolesHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoListRole, int>>();
        TodoMembersHelperService = Substitute.For<ITodoListDependantEntitiesService<TodoListMember, int>>();
        RefreshTokensRepository = Substitute.For<IUserRefreshTokensRepository>();
        PermissionsChecker = Substitute.For<IPermissionsChecker>();
        TodoListRolesRepository = Substitute.For<IRepository<TodoListRole, int>>();
        TodoListMembersRepository = Substitute.For<ITodoListMembersRepository>();

        UnitOfWork = Substitute.For<IUnitOfWork>();
        UnitOfWork.BeginTransactionAsync().Returns(Task.CompletedTask);
        UnitOfWork.CommitAsync().Returns(Task.CompletedTask);
        UnitOfWork.RollbackAsync().Returns(Task.CompletedTask);

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(_ => EntityExistenceChecker);
            services.AddScoped(_ => TodoListsRepository);
            services.AddScoped(_ => TodoItemsRepository);
            services.AddScoped(_ => TodoItemCategoriesRepository);
            services.AddScoped(_ => TestTodoListDependantEntitiesRepository);
            services.AddScoped(_ => InvitationLinksRepository);
            services.AddScoped(_ => TodoItemsHelperService);
            services.AddScoped(_ => TodoItemCategoriesHelperService);
            services.AddScoped(_ => TodoRolesHelperService);
            services.AddScoped(_ => TodoMembersHelperService);
            services.AddScoped(_ => RefreshTokensRepository);
            services.AddScoped(_ => PermissionsChecker);
            services.AddScoped(_ => TodoListRolesRepository);
            services.AddScoped(_ => TodoListMembersRepository);
            services.AddScoped(_ => UnitOfWork);
        });
    }
}
