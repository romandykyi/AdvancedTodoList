using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation;

/// <summary>
/// Validator class for <see cref="TodoListRoleCreateDto" />
/// </summary>
public class TodoListRoleCreateDtoValidator : AbstractValidator<TodoListRoleCreateDto>
{
    public TodoListRoleCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
        RuleFor(x => x.Name)
            .MaximumLength(TodoListRole.NameMaxLength)
            .WithErrorCode(ValidationErrorCodes.PropertyTooLong);

        RuleFor(x => x.Permissions)
            .NotNull()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
    }
}
