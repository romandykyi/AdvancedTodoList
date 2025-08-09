using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Validation;
using AdvancedTodoList.Application.Validation.Auth;

namespace AdvancedTodoList.UnitTests.Validation.Auth;

[TestFixture]
public class RegisterDtoValidatorTests
{
	private const string ValidEmail = "valid@example.com";
	private const string ValidFirstName = "Joe";
	private const string ValidLastName = "Doe";
	private const string ValidUserName = "joedoe123";
	private const string ValidPassword = "Pa$$w0rd";

	[Test]
	public void Dto_ValidDate_Succeeds()
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(ValidEmail, ValidUserName, ValidFirstName, ValidLastName, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void Email_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(testCase, ValidUserName, ValidFirstName, ValidLastName, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Email)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}

	[Test]
	[TestCase("@")]
	[TestCase("not@")]
	[TestCase("@mail")]
	[TestCase("mail.example.com")]
	public void Email_IsNotValid_ReturnsInvalidEmailError(string testCase)
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(testCase, ValidUserName, ValidFirstName, ValidLastName, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Email)
			.WithErrorCode(ValidationErrorCodes.InvalidEmail);
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void UserName_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(ValidEmail, testCase, ValidFirstName, ValidLastName, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.UserName)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void FirstName_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(ValidEmail, ValidUserName, testCase, ValidLastName, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.FirstName)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void LastName_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(ValidEmail, ValidUserName, ValidFirstName, testCase, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.LastName)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void Password_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		RegisterDtoValidator validator = new();
		RegisterDto dto = new(ValidEmail, ValidUserName, ValidFirstName, ValidLastName, testCase);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Password)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}
}
