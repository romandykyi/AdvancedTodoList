using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Validation;
using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.UnitTests.Validation;

[TestFixture]
public class TodoListCreateDtoValidatorTests
{
    private const string ValidName = "Valid";
    private const string ValidDescription = "Valid description";

    [Test]
    public void Dto_ValidData_Succeeds()
    {
        // Arrange
        TodoListCreateDtoValidator validator = new();
        TodoListCreateDto dto = new(ValidName, ValidDescription);

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
    public void Name_Empty_ReturnsPropertyRequiredError(string testCase)
    {
        // Arrange
        TodoListCreateDtoValidator validator = new();
        TodoListCreateDto dto = new(testCase, ValidDescription);

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
    }

    [Test]
    public void Name_TooLong_ReturnsPropertyTooLongError()
    {
        // Arrange
        TodoListCreateDtoValidator validator = new();
        string longName = new('X', TodoList.NameMaxLength + 1);
        TodoListCreateDto dto = new(longName, ValidDescription);

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ValidationErrorCodes.PropertyTooLong);
    }

    [Test]
    public void Description_Null_ReturnsPropertyRequiredError()
    {
        // Arrange
        TodoListCreateDtoValidator validator = new();
        TodoListCreateDto dto = new(ValidName, null!);

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorCode(ValidationErrorCodes.PropertyRequired);
    }

    [Test]
    [TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyNotNullStrings))]
    public void Description_EmptyAndNotNull_IsAllowed(string testCase)
    {
        // Arrange
        TodoListCreateDtoValidator validator = new();
        TodoListCreateDto dto = new(ValidName, testCase);

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Description_TooLong_ReturnsPropertyTooLongError()
    {
        // Arrange
        TodoListCreateDtoValidator validator = new();
        string longDescription = new('X', TodoList.DescriptionMaxLength + 1);
        TodoListCreateDto dto = new(ValidName, longDescription);

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorCode(ValidationErrorCodes.PropertyTooLong);
    }
}
