using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Validation;

namespace AdvancedTodoList.UnitTests.Validation;

[TestFixture]
public class TodoListRoleCreateDtoValidatorTests
{
	private const string ValidName = "Role";
	private readonly RolePermissions ValidPermissions = new(true, true, false, true);

	[Test]
	public void Dto_ValidData_Succeeds()
	{
		// Arrange
		TodoListRoleCreateDtoValidator validator = new();
		TodoListRoleCreateDto dto = new(ValidName, ValidPermissions);

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
		TodoListRoleCreateDtoValidator validator = new();
		TodoListRoleCreateDto dto = new(testCase, ValidPermissions);

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
		TodoListRoleCreateDtoValidator validator = new();
		string longName = new('X', TodoListRole.NameMaxLength + 1);
		TodoListRoleCreateDto dto = new(longName, ValidPermissions);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Name)
			.WithErrorCode(ValidationErrorCodes.PropertyTooLong);
	}

	[Test]
	public void Permissions_Null_ReturnsPropertyRequiredError()
	{
		// Arrange
		TodoListRoleCreateDtoValidator validator = new();
		TodoListRoleCreateDto dto = new(ValidName, null!);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Permissions)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}
}
