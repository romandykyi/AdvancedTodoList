using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.IntegrationTests.Services;

namespace AdvancedTodoList.IntegrationTests.Utils;

/// <summary>
/// A class that contains test models and IDs of entities which can be used
/// as dependencies.
/// </summary>
[SetUpFixture]
public static class TestModels
{
	/// <summary>
	/// Creates and returns a valid model of a to-do list which can be added to the DB. 
	/// </summary>
	public static TodoList CreateTestTodoList() => new()
	{
		Name = "Name",
		Description = "Description"
	};
	/// <summary>
	/// Creates and returns a valid model of a to-do list item which can be added to the DB. 
	/// </summary>
	public static TodoItem CreateTestTodoItem(string todoListId) => new()
	{
		Name = "Name",
		Description = "Description",
		State = TodoItemState.Completed,
		DeadlineDate = DateTime.UtcNow.AddDays(365),
		TodoListId = todoListId
	};
	/// <summary>
	/// Creates and returns a valid model of a test to-do list dependant entity. 
	/// </summary>
	public static TestTodoListDependantEntity CreateTestTodoListDependantEntity(string todoListId) => new()
	{
		TestProperty = "Test",
		TodoListId = todoListId
	};
	/// <summary>
	/// Creates and returns a valid model of an application user.
	/// </summary>
	public static ApplicationUser CreateTestUser()
	{
		string userName = Guid.NewGuid().ToString();
		return new()
		{
			UserName = userName,
			FirstName = "Test",
			LastName = "User",
			Email = $"{userName}@example.com"
		};
	}
	/// <summary>
	/// Creates and returns a valid model of a refresh token.
	/// </summary>
	public static UserRefreshToken CreateTestUserRefreshToken(string userId) => new()
	{
		UserId = userId,
		Token = "TestToken",
		ValidTo = DateTime.UtcNow.AddDays(180)
	};
	public static TodoListMemberRole CreateTestRole(string todoListId) => new()
	{
		Name = "Role1",
		TodoListId = todoListId
	};
}
