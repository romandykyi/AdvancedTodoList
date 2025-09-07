using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Application.Services.Implementations;
using AdvancedTodoList.Application.Services.Implementations.Auth;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Repositories;

namespace AdvancedTodoList.WebApp.Extensions.ServiceCollection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IRepository<TodoList, string>, TodoListRepository>()
            .AddScoped<IRepository<TodoItem, int>, TodoItemsRepository>()
            .AddScoped<IRepository<TodoItemCategory, int>, TodoItemCategoriesRepository>()
            .AddScoped<IRepository<InvitationLink, int>, InvitationLinksRepository>()
            .AddScoped<IInvitationLinksRepository, InvitationLinksRepository>()
            .AddScoped<ITodoListMembersRepository, TodoListMembersRepository>()
            .AddScoped<IRepository<TodoListMember, int>, TodoListMembersRepository>()
            .AddScoped<IRepository<TodoListRole, int>, TodoListRolesRepository>()
            .AddScoped<IRepository<UserRefreshToken, int>, UserRefreshTokensRepository>()
            .AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IPermissionsChecker, PermissionsChecker>()
            .AddScoped<ITodoListsService, TodoListsService>()
            .AddScoped<ITodoItemsService, TodoItemsService>()
            .AddScoped<ITodoItemCategoriesService, TodoItemCategoriesService>()
            .AddScoped<ITodoListMembersService, TodoListMembersService>()
            .AddScoped<ITodoListRolesService, TodoListRolesService>()
            .AddScoped<IInvitationLinksService, InvitationLinksService>()
            .AddScoped<IEntityExistenceChecker, EntityExistenceChecker>()
            .AddSingleton<IAccessTokensService, AccessTokensService>()
            .AddScoped<IRefreshTokensService, RefreshTokensService>()
            .AddScoped(typeof(ITodoListDependantEntitiesService<,>), typeof(TodoListDependantEntitiesService<,>));
    }
}
