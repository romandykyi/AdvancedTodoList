﻿using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Validation;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace AdvancedTodoList.UnitTests.Validation;

[TestFixture]
public class TodoItemCreateDtoValidatorTests
{
	private const string ValidName = "Valid";
	private const string ValidDescription = "Valid description";
	private static DateTime? ValidDeadline => DateTime.UtcNow + TimeSpan.FromDays(14);

	[Test]
	public void Dto_ValidData_Succeeds()
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, ValidDescription, ValidDeadline);

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
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(testCase, ValidDescription, ValidDeadline);

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
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, null!, ValidDeadline);

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
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, testCase, ValidDeadline);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldNotHaveValidationErrorFor(x => x.Description);
	}

	[Test]
	public void DeadlineDate_BeforeCurrentDate_ReturnsPropertyOutOfRangeError()
	{
		// Arrange
		TodoItemCreateDtoValidator validator = new();
		TodoItemCreateDto dto = new(ValidName, ValidDescription, DateTime.UtcNow - TimeSpan.FromDays(1));

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.DeadlineDate)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
	}
}