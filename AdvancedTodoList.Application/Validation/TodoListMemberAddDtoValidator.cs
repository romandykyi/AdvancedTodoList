using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Repositories;
using FluentValidation;

namespace AdvancedTodoList.Application.Validation;

/// <summary>
/// Validator class for <see cref="TodoListMemberAddDto" />
/// </summary>
public class TodoListMemberAddDtoValidator : AbstractValidator<TodoListMemberAddDto>
{
    public TodoListMemberAddDtoValidator(IEntityExistenceChecker existenceChecker)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);

        RuleFor(x => x.UserId)
            .MustAsync(async (userId, _) =>
            {
                return await existenceChecker.ExistsAsync<ApplicationUser, string>(userId);
            })
            .WithErrorCode(ValidationErrorCodes.InvalidForeignKey)
            .WithMessage("User not found.");
    }
}
