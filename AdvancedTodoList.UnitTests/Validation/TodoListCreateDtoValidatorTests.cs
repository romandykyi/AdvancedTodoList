using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Validation;

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
	[TestCase("")]
	[TestCase("    ")]
	[TestCase("\r\t \t\t\r")]
	[TestCase(null)]
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
	[TestCase("")]
	[TestCase("    ")]
	[TestCase("\r\t \t\t\r")]
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
}
