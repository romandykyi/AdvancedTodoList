using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.UnitTests.Validation;

namespace AdvancedTodoList.Core.Validation;

[TestFixture]
public class TodoListMemberAddDtoValidatorTests
{
	private TodoListMemberAddDtoValidator _validator;

	public const string ValidUserId = "ValidUserId";
	public const int ValidRoleId = 121;

	[SetUp]
	public void SetUp()
	{
		IEntityExistenceChecker existenceChecker = Substitute.For<IEntityExistenceChecker>();
		existenceChecker.ExistsAsync<ApplicationUser, string>(Arg.Any<string>())
			.Returns(false);
		existenceChecker.ExistsAsync<ApplicationUser, string>(ValidUserId)
			.Returns(true);
		_validator = new TodoListMemberAddDtoValidator(existenceChecker);
	}

	[Test]
	public async Task ValidDto_Succeeds()
	{
		// Arrange
		TodoListMemberAddDto dto = new(ValidUserId, ValidRoleId);

		// Act
		var result = await _validator.TestValidateAsync(dto);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	[TestCaseSource(typeof(ValidationConstants), nameof(ValidationConstants.EmptyStrings))]
	public async Task UserId_Empty_ReturnsPropertyRequiresError(string testCase)
	{
		// Arrange
		TodoListMemberAddDto dto = new(testCase, ValidRoleId);

		// Act
		var result = await _validator.TestValidateAsync(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.UserId)
			.WithErrorCode(ValidationErrorCodes.PropertyRequired);
	}

	[Test]
	public async Task UserId_Invalid_ReturnsInvalidForeignKeyError()
	{
		// Arrange
		TodoListMemberAddDto dto = new("Invalid", ValidRoleId);

		// Act
		var result = await _validator.TestValidateAsync(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.UserId)
			.WithErrorCode(ValidationErrorCodes.InvalidForeignKey);
	}

	[Test]
	public async Task RoleId_Null_IsAccepted()
	{
		// Arrange
		TodoListMemberAddDto dto = new(ValidUserId, null);

		// Act
		var result = await _validator.TestValidateAsync(dto);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}
}
