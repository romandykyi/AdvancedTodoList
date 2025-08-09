using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Specifications;

namespace AdvancedTodoList.UnitTests.Specifications;

[TestFixture]
public class GetByIdSpecificationTests
{
	private class TestEntity : IEntity<int>
	{
		public int Id { get; set; }
	}

	[Test]
	public void Criteria_IdMatches_ReturnsTrue()
	{
		// Arrange
		int testId = 123;
		TestEntity entity = new() { Id = testId };
		GetByIdSpecification<TestEntity, int> specification = new(testId);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_IdDoesNotMatch_ReturnsFalse()
	{
		// Arrange
		int testId = 123;
		TestEntity entity = new() { Id = testId };
		GetByIdSpecification<TestEntity, int> specification = new(-1);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(entity);

		// Assert
		Assert.That(result, Is.False);
	}
}
