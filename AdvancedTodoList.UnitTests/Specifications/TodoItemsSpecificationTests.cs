using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications.Filters;
using AdvancedTodoList.Core.Specifications.Todo;

namespace AdvancedTodoList.UnitTests.Specifications;

[TestFixture]
public class TodoItemsSpecificationTests
{
    private TodoItem _testItem;

    [SetUp]
    public void SetUp()
    {
        _testItem = new()
        {
            Id = 700,
            Name = "Make the call",
            Description = "Call someone special to fix your problems",
            OwnerId = "TestOwnerId",
            TodoListId = "TestListId",
            Priority = 555,
            CategoryId = 777,
            DeadlineDate = new(2022, 8, 15),
            State = TodoItemState.Completed
        };
    }

    [Test]
    public void Criteria_TodoListIdMatchesEmptyFilter_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsSpecification specification = new(todoListId, new());
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_TodoListIdDoesNotMatch_ReturnsFalse()
    {
        // Arrange
        TodoItemsSpecification specification = new("Wrong ID", new());
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_SubstringOfName_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(Name: _testItem.Name[5..7]);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_NotSubstringOfName_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(Name: "Not a substring");
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_OwnerId_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(OwnerId: _testItem.OwnerId);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_NotSubstringOfOwnerId_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(OwnerId: "WrongOwnerId");
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_SubsetOfState_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemState[] states = [TodoItemState.Completed, TodoItemState.Active, TodoItemState.Skipped];
        TodoItemsFilter filter = new(State: states);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_NotSubsetOfState_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemState[] states = [TodoItemState.Skipped];
        TodoItemsFilter filter = new(State: states);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_SubsetOfCategoryId_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        int?[] categoriesIds = [null, _testItem.CategoryId, 1, 10000];
        TodoItemsFilter filter = new(CategoryId: categoriesIds);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_NotSubsetOfCategoryId_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        int?[] categoriesIds = [null, 1, 100, 200];
        TodoItemsFilter filter = new(CategoryId: categoriesIds);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_PriorityIsGreaterThanMinPriotity_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MinPriority: _testItem.Priority - 1);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_PriorityIsLessThanMinPriority_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MinPriority: _testItem.Priority + 1);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_PriorityIsGreaterThanMaxPriotity_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MaxPriority: _testItem.Priority + 1);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_PriorityIsGreaterThanMaxPriority_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MaxPriority: _testItem.Priority - 1);
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_DeadlineDateIsGreaterThanMinPriotity_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MinDeadlineDate: _testItem.DeadlineDate!.Value.AddDays(-1));
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_DeadlineDateIsLessThanMinDeadlineDate_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MinDeadlineDate: _testItem.DeadlineDate!.Value.AddDays(1));
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Criteria_DeadlineDateIsGreaterThanMaxPriotity_ReturnsTrue()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MaxDeadlineDate: _testItem.DeadlineDate!.Value.AddDays(1));
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Criteria_DeadlineDateIsGreaterThanMaxDeadlineDate_ReturnsFalse()
    {
        // Arrange
        string todoListId = _testItem.TodoListId;
        TodoItemsFilter filter = new(MaxDeadlineDate: _testItem.DeadlineDate!.Value.AddDays(-1));
        TodoItemsSpecification specification = new(todoListId, filter);
        var function = specification.Criteria.Compile();

        // Act
        bool result = function(_testItem);

        // Assert
        Assert.That(result, Is.False);
    }
}
