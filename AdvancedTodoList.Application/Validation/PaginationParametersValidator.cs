using AdvancedTodoList.Core.Pagination;
using FluentValidation;

namespace AdvancedTodoList.Core.Validation;

public class PaginationParametersValidator : AbstractValidator<PaginationParameters>
{
	public PaginationParametersValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThan(0)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange)
			.WithMessage("Page number must be a positive number.");

		RuleFor(x => x.PageSize)
			.GreaterThanOrEqualTo(PaginationParameters.MinPageSize)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange)
			.WithMessage($"Page size must be within the range of {PaginationParameters.MinPageSize}-{PaginationParameters.MaxPageSize}");

		RuleFor(x => x.PageSize)
			.LessThanOrEqualTo(PaginationParameters.MaxPageSize)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange)
			.WithMessage($"Page size must be within the range of {PaginationParameters.MinPageSize}-{PaginationParameters.MaxPageSize}");
	}
}
