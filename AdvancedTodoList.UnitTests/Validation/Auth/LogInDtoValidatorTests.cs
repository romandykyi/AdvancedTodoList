using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Validation;
using AdvancedTodoList.Core.Validation.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTodoList.UnitTests.Validation.Auth;

public class LogInDtoValidatorTests
{
	private const string ValidUserNameOrEmail = "username";
	private const string ValidPassword = "Pa$$w0rd";

	[Test]
	public void Dto_ValidData_Succeeds()
	{
		// Arrange
		LogInDtoValidator validator = new();
		LogInDto dto = new(ValidUserNameOrEmail, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void UserNameOrEmail_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		LogInDtoValidator validator = new();
		LogInDto dto = new(testCase, ValidPassword);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.UserNameOrEmail)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public void Password_Empty_ReturnsPropertyRequiredError(string testCase)
	{
		// Arrange
		LogInDtoValidator validator = new();
		LogInDto dto = new(ValidUserNameOrEmail, testCase);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Password)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}
}
