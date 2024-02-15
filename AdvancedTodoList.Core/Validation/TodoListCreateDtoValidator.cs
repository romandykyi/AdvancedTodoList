using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using FluentValidation;

namespace AdvancedTodoList.Core.Validation;

public class TodoListCreateDtoValidator : AbstractValidator<TodoListCreateDto>
{
	public TodoListCreateDtoValidator()
	{
		// Name is required
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);

		// Name should not be too long
		RuleFor(x => x.Name)
			.MaximumLength(TodoList.NameMaxLength)
			.WithErrorCode(ValidationErrorCodes.PropertyTooLong);

		// Description is not null
		RuleFor(x => x.Description)
			.NotNull()
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);

		// Description should not be too long
		RuleFor(x => x.Description)
			.MaximumLength(TodoList.DescriptionMaxLength)
			.WithErrorCode(ValidationErrorCodes.PropertyTooLong);
	}
}
