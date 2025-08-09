using AdvancedTodoList.Application.Validation;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.UnitTests.Validation;

[TestFixture]
public class PaginationParametersValidatorTests
{
    private const int ValidPage = 4;
    private const int ValidPageSize = 20;

    [Test]
    public void Parameters_Valid_Succeeds()
    {
        // Arrange
        PaginationParametersValidator validator = new();
        PaginationParameters parameters = new(ValidPage, ValidPageSize);

        // Act
        var result = validator.TestValidate(parameters);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void Page_OutOfRange_ReturnsPropertyOutOfRangeError(int testCase)
    {
        // Arrange
        PaginationParametersValidator validator = new();
        PaginationParameters parameters = new(testCase, ValidPageSize);

        // Act
        var result = validator.TestValidate(parameters);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(1_000_000)]
    [TestCase(PaginationParameters.MinPageSize - 1)]
    [TestCase(PaginationParameters.MaxPageSize + 1)]
    public void PageSize_OutOfRange_ReturnsPropertyOutOfRangeError(int testCase)
    {
        // Arrange
        PaginationParametersValidator validator = new();
        PaginationParameters parameters = new(ValidPage, testCase);

        // Act
        var result = validator.TestValidate(parameters);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
    }
}
