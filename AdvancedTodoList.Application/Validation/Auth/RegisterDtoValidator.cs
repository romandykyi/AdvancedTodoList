using AdvancedTodoList.Application.Dtos;
using FluentValidation;
using FluentValidation.Validators;

namespace AdvancedTodoList.Application.Validation.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
        RuleFor(x => x.Email)
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
            .WithErrorCode(ValidationErrorCodes.InvalidEmail);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
    }
}
