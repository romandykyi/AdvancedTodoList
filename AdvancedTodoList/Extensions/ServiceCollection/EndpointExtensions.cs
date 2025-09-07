using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AdvancedTodoList.WebApp.Extensions.ServiceCollection;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        return services.AddEndpointsApiExplorer();
    }

    public static IServiceCollection AddApplicationSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter access token (JWT)",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public static IServiceCollection AddApplicationAntiforgery(this IServiceCollection services)
    {
        // Configure antiforgery
        return services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");
    }
}
