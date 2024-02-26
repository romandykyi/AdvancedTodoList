using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

[TestFixture]
public class TodoListRepositoryTests : BaseRepositoryTests<TodoList, string>
{
	protected override string NonExistingId => string.Empty;

	protected const string UpdatedName = "update";
	protected const string UpdatedDescription = "new";

	protected override void UpdateEntity(TodoList entity)
	{
		entity.Name = UpdatedName;
		entity.Description = UpdatedDescription;
	}

	protected override void AssertUpdated(TodoList updatedEntity)
	{
		Assert.Multiple(() =>
		{
			Assert.That(updatedEntity.Name, Is.EqualTo(UpdatedName));
			Assert.That(updatedEntity.Description, Is.EqualTo(UpdatedDescription));
		});
	}

	protected override Task<TodoList> CreateTestEntityAsync()
	{
		return Task.FromResult(TestModels.CreateTestTodoList());
	}
}
