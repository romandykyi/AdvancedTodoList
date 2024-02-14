using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Validation;

namespace AdvancedTodoList.UnitTests.Validation;

[TestFixture]
public class TodoItemUpdateStateDtoValidatorTests
{
	[Test]
	[TestCase(TodoItemState.Active)]
	[TestCase(TodoItemState.Completed)]
	[TestCase(TodoItemState.Skipped)]
	public void State_Valid_Succeeds(TodoItemState testCase)
	{
		// Arrange
		TodoItemUpdateStateDtoValidator validator = new();
		TodoItemUpdateStateDto dto = new(testCase);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	[TestCase((TodoItemState)6)]
	[TestCase((TodoItemState)124)]
	[TestCase((TodoItemState)200)]
	public void State_OutOfRange_ReturnsPropertyOutOfRangeError(TodoItemState testCase)
	{
		// Arrange
		TodoItemUpdateStateDtoValidator validator = new();
		TodoItemUpdateStateDto dto = new(testCase);

		// Act
		var result = validator.TestValidate(dto);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.State)
			.WithErrorCode(ValidationErrorCodes.PropertyOutOfRange);
	}
}
