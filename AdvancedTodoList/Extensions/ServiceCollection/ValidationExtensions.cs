using AdvancedTodoList.Application.Validation;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace AdvancedTodoList.WebApp.Extensions.ServiceCollection;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false; // Disable localization
        return services.AddValidatorsFromAssemblyContaining<TodoItemCreateDtoValidator>();
    }

    public static IServiceCollection AddFluentValidationAutoValidation(this IServiceCollection services)
    {
        return services.AddFluentValidationAutoValidation(configuration =>
        {
            // Disable the built-in .NET model (data annotations) validation.
            configuration.DisableBuiltInModelValidation = true;
            // Enable validation for parameters bound from 'BindingSource.Body' binding sources.
            configuration.EnableBodyBindingSourceAutomaticValidation = true;
            // Enable validation for parameters bound from 'BindingSource.Query' binding sources.
            configuration.EnableQueryBindingSourceAutomaticValidation = true;
        });
    }
}
