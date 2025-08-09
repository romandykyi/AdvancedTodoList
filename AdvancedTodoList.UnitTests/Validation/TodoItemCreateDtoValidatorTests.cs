using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Validation;
using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.UnitTests.Validation;

[TestFixture]
public class TodoItemCreateDtoValidatorTests
{
	private const string ValidName = "Valid";
	private const string ValidDescription = "Valid description";
	private const int ValidPriority = 4;
	private const int ValidCategoryId = 3;
	private static DateTime? ValidDeadline => DateTime.UtcNow + TimeSpan.FromDays(14);

	[Test]
	public void Dto_ValidData_Succeeds()
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, ValidDescription, ValidDeadline, ValidPriority, ValidCategoryId);

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
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(testCase, ValidDescription, ValidDeadline, ValidPriority, ValidCategoryId);

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
		TodoItemCreateDtoValidator validator = new();
		string longName = new('X', TodoItem.NameMaxLength + 1);
		TodoItemCreateDto dto = new(longName, ValidDescription, ValidDeadline, ValidPriority, ValidCategoryId);

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
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, null!, ValidDeadline, ValidPriority, ValidCategoryId);

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
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, testCase, ValidDeadline, ValidPriority, ValidCategoryId);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldNotHaveValidationErrorFor(x => x.Description);
	}

	[Test]
	public void Description_TooLong_ReturnsPropertyTooLongError()
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		string longDescription = new('X', TodoItem.DescriptionMaxLength + 1);
		TodoItemCreateDto dto = new(ValidName, longDescription, ValidDeadline, ValidPriority, ValidCategoryId);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Description)
			.WithErrorCode(ValidationErrorCodes.PropertyTooLong);
	}

	[Test]
	public void DeadlineDate_BeforeCurrentDate_ReturnsPropertyOutOfRangeError()
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, ValidDescription,
			DateTime.UtcNow - TimeSpan.FromDays(1), ValidPriority, ValidCategoryId);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.DeadlineDate)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
	}

	[Test]
	[TestCase(-1)]
	[TestCase(11)]
	public void Priority_OutOfRange_ReturnsPropertyOutOfRangeError(int testCase)
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, ValidDescription,
			ValidDeadline, testCase, ValidCategoryId);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Priority)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
	}

	[Test]
	public void CategoryId_Null_Allowed()
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, ValidDescription,
			ValidDeadline, ValidPriority, null);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
	}
}
