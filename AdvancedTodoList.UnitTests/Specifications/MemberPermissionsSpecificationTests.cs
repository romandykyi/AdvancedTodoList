using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Specifications;

namespace AdvancedTodoList.UnitTests.Specifications;

public class MemberPermissionsSpecificationTests
{
    [Test]
    public void Criteria_ValidTodoListIdAndUserId_ReturnsTrue()
    {
        // Arrange
        const string todoListId = "TodoId";
        const string userId = "UserId";
        TodoListMember member = new()
        {
            TodoListId = todoListId,
            UserId = userId
        };
        MemberPermissionsSpecification specification = new(todoListId, userId);
        var criteria = specification.Criteria.Compile();

        // Act
        bool result = criteria(member);

        // Arrange
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase("TodoListId", "invalid")]
    [TestCase("invalid", "UserId")]
    [TestCase("invalid", "invalid")]
    public void Criteria_InvalidTodoListIdAndUserIdPairs_ReturnsFalse(string todoListId, string userId)
    {
        // Arrange
        TodoListMember member = new()
        {
            TodoListId = "TodoListId",
            UserId = "UserId"
        };
        MemberPermissionsSpecification specification = new(todoListId, userId);
        var criteria = specification.Criteria.Compile();

        // Act
        bool result = criteria(member);

        // Arrange
        Assert.That(result, Is.False);
    }
}
