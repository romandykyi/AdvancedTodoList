using FluentValidation;
using AdvancedTodoList.Core.Dtos;

namespace AdvancedTodoList.Core.Validation;

public class TodoListCreateDtoValidator : AbstractValidator<TodoListCreateDto>
{
	public TodoListCreateDtoValidator()
	{
		// Name is required
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);

		// Description is not null
		RuleFor(x => x.Description)
			.NotNull()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}
}
