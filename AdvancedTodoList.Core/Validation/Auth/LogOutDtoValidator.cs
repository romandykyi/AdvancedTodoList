using AdvancedTodoList.Core.Dtos;
using FluentValidation;

namespace AdvancedTodoList.Core.Validation.Auth;

public class LogOutDtoValidator : AbstractValidator<LogOutDto>
{
	public LogOutDtoValidator()
	{
		RuleFor(x => x.RefreshToken)
			.NotEmpty()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}
}
