using AdvancedTodoList.Core.Dtos;
using FluentValidation;

namespace AdvancedTodoList.Core.Validation;

/// <summary>
/// Validator class for <see cref="TodoItemCreateDto" />
/// </summary>
public class TodoItemCreateDtoValidator : AbstractValidator<TodoItemCreateDto>
{
	public TodoItemCreateDtoValidator()
	{
		// Name is required
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);

		// Description is not null
		RuleFor(x => x.Description)
			.NotNull()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);

		// Deadline date should be after the current date
		RuleFor(x => x.DeadlineDate)
			.GreaterThan(DateTime.UtcNow)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
	}
}
