using AdvancedTodoList.Core.Dtos;
using FluentValidation;

namespace AdvancedTodoList.Core.Validation.Auth;

public class LogInDtoValidator : AbstractValidator<LogInDto>
{
	public LogInDtoValidator()
	{
		RuleFor(x => x.UserNameOrEmail)
			.NotEmpty()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
		RuleFor(x => x.Password)
			.NotEmpty()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}
}
