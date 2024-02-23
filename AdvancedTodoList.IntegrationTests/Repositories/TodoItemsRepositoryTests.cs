using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

public class TodoItemsRepositoryTests : BaseRepositoryTests<TodoItem, int>
{
	protected override int NonExistingId => -1;

	protected const string UpdatedName = "New name";
	protected const string UpdatedDescription = "New description";
	protected const TodoItemState UpdatedState = TodoItemState.Completed;
	protected readonly DateTime UpdatedDeadlineDate = DateTime.UtcNow.AddDays(100);

	protected override void UpdateEntity(TodoItem entity)
	{
		entity.Name = UpdatedName;
		entity.Description = UpdatedDescription;
		entity.State = UpdatedState;
		entity.DeadlineDate = UpdatedDeadlineDate;
	}

	protected override void AssertUpdated(TodoItem updatedEntity)
	{
		Assert.Multiple(() =>
		{
			Assert.That(updatedEntity.Name, Is.EqualTo(UpdatedName));
			Assert.That(updatedEntity.Description, Is.EqualTo(UpdatedDescription));
			Assert.That(updatedEntity.State, Is.EqualTo(UpdatedState));
			Assert.That(updatedEntity.DeadlineDate, Is.EqualTo(UpdatedDeadlineDate));
		});
	}

	protected override async Task<TodoItem> CreateTestEntityAsync()
	{
		var todoList = TestModels.CreateTestTodoList();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		return TestModels.CreateTestTodoItem(todoList.Id);
	}
}
