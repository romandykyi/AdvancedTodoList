using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.UnitTests.Specifications;

[TestFixture]
public class TodoListMembersSpecificationTests
{
	private TodoListMember _testMember;

	[SetUp]
	public void SetUp()
	{
		ApplicationUser user = new()
		{
			Id = "TestUserId",
			UserName = "TestUserName",
			FirstName = "TestFirstName",
			LastName = "TestLastName"
		};
		_testMember = new()
		{
			Id = 123,
			TodoListId = "TestTodoListId",
			User = user,
			UserId = user.Id,
			RoleId = 777
		};
	}

	[Test]
	public void Criteria_TodoListIdMatchesEmptyFilter_ReturnsTrue()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new());
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_TodoListIdDoesNotMatch_ReturnsFalse()
	{
		// Arrange
		TodoListMembersSpecification specification = new("Wrong ID", new());
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void Criteria_SubsetOfRolesIds_ReturnsTrue()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		int?[] roleIds = [null, 1, _testMember.RoleId, 2];
		TodoListMembersSpecification specification = new(todoListId, new(roleIds));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_NotSubsetOfRolesIds_ReturnsFalse()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		int?[] roleIds = [null, 2];
		TodoListMembersSpecification specification = new(todoListId, new(roleIds));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void Criteria_UserIdMatches_ReturnsTrue()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new(UserId: _testMember.UserId));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_UserIdMatches_ReturnsFalse()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new(UserId: "wrong id"));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void Criteria_SubstringOfUserName_ReturnsTrue()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new(
			UserName: _testMember.User.UserName![1..4]));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_NotSubstringOfUserName_ReturnsFalse()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new(
			UserName: "wrong-username"));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void Criteria_SubstringOfFullName_ReturnsTrue()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new(
			FullName: _testMember.User.LastName![1..3]));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void Criteria_NotSubstringOfFullName_ReturnsFalse()
	{
		// Arrange
		string todoListId = _testMember.TodoListId;
		TodoListMembersSpecification specification = new(todoListId, new(
			FullName: "wrong-username"));
		var function = specification.Criteria.Compile();

		// Act
		bool result = function(_testMember);

		// Assert
		Assert.That(result, Is.False);
	}
}
