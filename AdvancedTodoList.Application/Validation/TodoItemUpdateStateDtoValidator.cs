using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation;

public class TodoItemUpdateStateDtoValidator : AbstractValidator<TodoItemUpdateStateDto>
{
    public TodoItemUpdateStateDtoValidator()
    {
        RuleFor(x => x.State)
            .Must(s => s >= TodoItemState.Active && s <= TodoItemState.Skipped)
            .WithErrorCode(ValidationErrorCodes.PropertyOutOfRange)
            .WithMessage(x => $"{(int)x.State} is invalid value for {{PropertyName}}");
    }
}
