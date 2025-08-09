using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications;

namespace AdvancedTodoList.UnitTests.Specifications;

[TestFixture]
public class InvitationLinksSpecificationTests
{
    [Test]
    public void Criteria_TodoListIdMatches_ReturnsTrue()
    {
        // Arrange
        const string todoListId = "ID";
        InvitationLink link = new() { TodoListId = todoListId, Value = "link" };
        InvitationLinksSpecification specification = new(todoListId);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(link);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_TodoListIdDoesNotMatch_ReturnsFalse()
    {
        // Arrange
        const string todoListId = "ID";
        InvitationLink link = new() { TodoListId = todoListId, Value = "link" };
        InvitationLinksSpecification specification = new("Wrong ID");
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(link);

        // Assert
        Assert.That(result, Is.False);
    }
}
