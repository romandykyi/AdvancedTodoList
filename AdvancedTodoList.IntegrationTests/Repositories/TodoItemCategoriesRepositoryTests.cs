using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

[TestFixture]
public class TodoItemCategoriesRepositoryTests : BaseRepositoryTests<TodoItemCategory, int>
{
	protected override int NonExistingId => -1;
	protected const string UpdatedName = "New name";

	protected override void AssertUpdated(TodoItemCategory updatedEntity)
	{
		Assert.That(updatedEntity.Name, Is.EqualTo(UpdatedName));
	}

	protected override async Task<TodoItemCategory> CreateTestEntityAsync()
	{
		var todoList = TestModels.CreateTestTodoList();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		return TestModels.CreateTestTodoItemCategory(todoList.Id);
	}

	protected override void UpdateEntity(TodoItemCategory entity)
	{
		entity.Name = UpdatedName;
	}
}
