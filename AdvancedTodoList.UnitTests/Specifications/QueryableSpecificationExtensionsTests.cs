using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace AdvancedTodoList.UnitTests.Specifications;

[TestFixture]
public class QueryableSpecificationExtensionsTests
{
    private class TestSpecification<T> : ISpecification<T>
        where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; set; } = _ => true;

        public List<Expression<Func<T, object?>>> Includes { get; set; } = [];

        public List<string> IncludeStrings { get; set; } = [];
    }

    [Test]
    public void ApplySpecification_AppliesCriteria()
    {
        // Arrange
        TestSpecification<string> specification = new()
        {
            Criteria = s => s[0] == 'a'
        };
        string[] testArray = ["abc", "hab", "bcd", "aab"];
        string[] expectedResult = ["abc", "aab"];
        IQueryable<string> testQuery = testArray.AsQueryable();

        // Act
        var result = testQuery.ApplySpecification(specification);

        // Assert
        Assert.That(result, Is.EquivalentTo(expectedResult));
    }
}
