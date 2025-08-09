using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation;

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

        // Name should not be too long
        RuleFor(x => x.Name)
            .MaximumLength(TodoItem.NameMaxLength)
            .WithErrorCode(ValidationErrorCodes.PropertyTooLong);

        // Description is not null
        RuleFor(x => x.Description)
            .NotNull()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        // Description should not be too long
        RuleFor(x => x.Description)
            .MaximumLength(TodoItem.DescriptionMaxLength)
            .WithErrorCode(ValidationErrorCodes.PropertyTooLong);

        // Deadline date should be after the current date
        RuleFor(x => x.DeadlineDate)
            .GreaterThan(DateTime.UtcNow)
            .WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);

        // Priority must be in a specific range
        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(TodoItem.MinPriority)
            .WithErrorCode(ValidationErrorCodes.PropertyOutOfRange)
            .WithMessage($"Priority must be within the range from {TodoItem.MinPriority} to {TodoItem.MaxPriority}");
        RuleFor(x => x.Priority)
            .LessThanOrEqualTo(TodoItem.MaxPriority)
            .WithErrorCode(ValidationErrorCodes.PropertyOutOfRange)
            .WithMessage($"Priority must be within the range from {TodoItem.MinPriority} to {TodoItem.MaxPriority}");
    }
}
