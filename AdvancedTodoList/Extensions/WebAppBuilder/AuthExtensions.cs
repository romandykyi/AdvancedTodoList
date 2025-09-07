using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdvancedTodoList.WebApp.Extensions.WebAppBuilder;

public static class AuthExtensions
{
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        // Configure auth
        string? jwtSecret = builder.Configuration["Auth:AccessToken:SecretKey"] ??
            throw new InvalidOperationException("JWT secret is not configured");
        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromSeconds(5),
                    ValidIssuer = builder.Configuration["Auth:AccessToken:ValidIssuer"],
                    ValidAudience = builder.Configuration["Auth:AccessToken:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });
        builder.Services.AddAuthorizationBuilder();

        return builder;
    }
}
