using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation;

public class TodoItemCategoryCreateDtoValidator : AbstractValidator<TodoItemCategoryCreateDto>
{
    public TodoItemCategoryCreateDtoValidator()
    {
        // Name is required
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        // Name should not be too long
        RuleFor(x => x.Name)
            .MaximumLength(TodoItemCategory.NameMaxLength)
            .WithErrorCode(ValidationErrorCodes.PropertyTooLong);
    }
}
