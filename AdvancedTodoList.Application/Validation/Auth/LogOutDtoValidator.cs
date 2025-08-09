using AdvancedTodoList.Application.Dtos;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation.Auth;

public class LogOutDtoValidator : AbstractValidator<LogOutDto>
{
    public LogOutDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
    }
}
