using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.UnitTests.Specifications;

public class TodoListDependantEntitiesSpecificationTests
{
	private class TestEntity : ITodoListDependant
	{
		public required string TodoListId { get; set; }
	}

	[Test]
	public void Criteria_TodoListIdMatches_ReturnsTrue()
	{
		// Arrange
		string todoListId = "ID";
		TestEntity entity = new() { TodoListId = todoListId };
		TodoListDependantEntitiesSpecification<TestEntity> specification = new(todoListId);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_TodoListIdDoesNotMatch_ReturnsFalse()
	{
		// Arrange
		TestEntity entity = new() { TodoListId = "TestId" };
		TodoListDependantEntitiesSpecification<TestEntity> specification = new("Wrong ID");
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.False);
	}
}
