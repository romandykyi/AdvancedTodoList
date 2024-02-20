using AdvancedTodoList.Infrastructure.Data;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Setup;

/// <summary>
/// A class that can be used to populate a database before running integration tests.
/// </summary>
public static class DbPopulator
{
	/// <summary>
	/// An ID of the to-do list which exists in the DB (can be used as a dependency).
	/// </summary>
	public static string ExistingTodoListId { get; set; } = null!;

	public const int TodoListsCount = 3;
	public const int TodoListsItemsPerList = 5;

	public static async Task PopulateDbAsync(ApplicationDbContext dbContext)
	{
		for (int i = 0; i < TodoListsCount; i++)
		{
			// Add a test to-do list
			var list = TestModels.CreateTestTodoList();
			dbContext.Add(list);
			// Save it
			await dbContext.SaveChangesAsync();
			// Set ID for dependencies
			ExistingTodoListId = list.Id;

			// Add test to-do list items for it
			for (int j = 0; j < TodoListsItemsPerList; j++)
			{
				dbContext.Add(TestModels.CreateTestTodoItem(list.Id));
			}
			// Save items
			await dbContext.SaveChangesAsync();
		}
	}
}
