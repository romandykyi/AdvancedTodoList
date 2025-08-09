using AdvancedTodoList.Application.Dtos;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation.Auth;

public class RefreshDtoValidator : AbstractValidator<RefreshDto>
{
    public RefreshDtoValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
    }
}
