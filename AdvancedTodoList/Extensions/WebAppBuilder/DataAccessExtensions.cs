using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.WebApp.Extensions.WebAppBuilder;

public static class DataAccessExtensions
{
    public static WebApplicationBuilder AddDataAccess(this WebApplicationBuilder builder)
    {
        // Get connection string
        string? connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string is not specified in 'ConnectionStrings:DefaultConnection'");

        // Configure DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(
            o => o.UseSqlServer(connectionString,
            b => b.MigrationsAssembly("AdvancedTodoList.Infrastructure"))
            );

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        return builder;
    }
}
