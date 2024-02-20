using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.IntegrationTests.Setup;

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
}
