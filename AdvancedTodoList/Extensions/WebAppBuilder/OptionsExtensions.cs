using AdvancedTodoList.Application.Options;

namespace AdvancedTodoList.WebApp.Extensions.WebAppBuilder;

public static class OptionsExtensions
{
    public static WebApplicationBuilder ConfigureApplicationOptions(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<AccessTokenOptions>(
                builder.Configuration.GetSection("Auth:AccessToken"))
            .Configure<RefreshTokenOptions>(
                builder.Configuration.GetSection("Auth:RefreshToken"))
            .Configure<InvitationLinkOptions>(
                builder.Configuration.GetSection("Todo:InvitationLink")
                );

        return builder;
    }
}
