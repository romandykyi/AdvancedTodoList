using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications.Todo;

namespace AdvancedTodoList.UnitTests.Specifications;

public class TodoListDependantEntitiesSpecificationTests
{
	private class TestEntity : ITodoListDependant, IHasName
	{
		public required string TodoListId { get; set; }
		public required string Name { get; set; }
	}

	[Test]
	public void Criteria_TodoListIdMatches_ReturnsTrue()
	{
		// Arrange
		const string todoListId = "ID";
		TestEntity entity = new() { TodoListId = todoListId, Name = "Name" };
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
		const string todoListId = "ID";
		TestEntity entity = new() { TodoListId = todoListId, Name = "Name" };
		TodoListDependantEntitiesSpecification<TestEntity> specification = new("Wrong ID");
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void Criteria_NameMatches_ReturnsTrue()
	{
		// Arrange
		TestEntity entity = new() { TodoListId = "ID", Name = "Name" };
		TodoListDependantEntitiesSpecification<TestEntity> specification = new(
			entity.TodoListId, entity.Name[1..3]);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_NameDoesNotMatch_ReturnsFalse()
	{
		// Arrange
		TestEntity entity = new() { TodoListId = "ID", Name = "Name" };
		TodoListDependantEntitiesSpecification<TestEntity> specification = new(
			entity.TodoListId, "Wrong");
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.False);
	}
}
