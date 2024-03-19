using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.UnitTests.Specifications;

[TestFixture]
public class TodoListsSpecificationTests
{
	private TodoList _testList;
	private const string TestUserId = "TestUserId";

	[SetUp]
	public void SetUp()
	{
		string id = "aajaogdsnbvoain";
		TodoListMember[] members =
		{
			new () {TodoListId = id, UserId = "123"},
			new () {TodoListId = id, UserId = TestUserId},
			new () {TodoListId = id, UserId = "321"},
		};
		_testList = new TodoList()
		{
			Name = "Test list",
			Description = "Description",
			TodoListMembers = members,
			OwnerId = "1227"
		};
	}

	[Test]
	public void Criteria_UserIsMemberOfList_ReturnsTrue()
	{
		// Arrange
		TodoListsFilter filter = new();
		TodoListsSpecification specification = new(TestUserId, filter);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testList);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_UserIsMemberOfList_ReturnsFalse()
	{
		// Arrange
		TodoListsFilter filter = new();
		TodoListsSpecification specification = new("Wrong user ID", filter);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testList);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void Criteria_NameMatches_ReturnsTrue()
	{
		// Arrange
		TodoListsFilter filter = new(Name: _testList.Name[2..6]);
		TodoListsSpecification specification = new(TestUserId, filter);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testList);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_NameDoesNotMatch_ReturnsFalse()
	{
		// Arrange
		TodoListsFilter filter = new(Name: "Name that does not match");
		TodoListsSpecification specification = new(TestUserId, filter);
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testList);

		// Assert
		Assert.That(result, Is.False);
	}
}
