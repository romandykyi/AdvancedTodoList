using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Infrastructure.Pagination;

namespace AdvancedTodoList.UnitTests.Pagination;

public class QueryablePaginationExtensionsTests
{
	[Test]
	public void ApplyPagination_ValidPage_ReturnsValidResults()
	{
		// Arrange
		var data = Enumerable.Range(1, 50).AsQueryable();
		PaginationParameters properties = new(2, 10);

		// Act
		var page = data.ApplyPagination(properties);

		// Assert
		Assert.That(page, Is.EquivalentTo(Enumerable.Range(11, 10)));
	}

	[Test]
	public void ApplyPagination_WrongPage_ReturnsEmptyResults()
	{
		// Arrange
		var data = Enumerable.Range(1, 50).AsQueryable();
		PaginationParameters properties = new(111, 10);

		// Act
		var page = data.ApplyPagination(properties);

		// Assert
		Assert.That(page, Is.Empty);
	}
}
