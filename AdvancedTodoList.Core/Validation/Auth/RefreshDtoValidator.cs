using AdvancedTodoList.Core.Dtos;
using FluentValidation;

namespace AdvancedTodoList.Core.Validation.Auth;

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
