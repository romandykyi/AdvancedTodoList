using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Specifications.Filters;
using AdvancedTodoList.Core.Specifications.Todo;
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

	private async Task<TodoItem> CreateItemWithOwnerAndCategoryAsync()
	{
		var owner = TestModels.CreateTestUser();
		DbContext.Add(owner);
		await DbContext.SaveChangesAsync();
		var todoList = TestModels.CreateTestTodoList();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		var category = TestModels.CreateTestTodoItemCategory(todoList.Id);
		DbContext.Add(category);
		await DbContext.SaveChangesAsync();
		var todoItem = TestModels.CreateTestTodoItem(todoList.Id, owner.Id, category.Id);
		DbContext.Add(todoItem);
		await DbContext.SaveChangesAsync();

		return todoItem;
	}

	[Test]
	public async Task GetAggregateAsync_TodoListAggregateSpecification_IncludesOwnerAndCategory()
	{
#pragma warning disable NUnit2045 // Use Assert.Multiple
		// Arrange
		var todoItem = await CreateItemWithOwnerAndCategoryAsync();
		TodoItemAggregateSpecification specification = new(todoItem.Id);

		// Act
		var aggregate = await Repository.GetAggregateAsync<TodoItemGetByIdDto>(specification);

		// Assert
		Assert.That(aggregate, Is.Not.Null);
		Assert.That(aggregate.Owner, Is.Not.Null);
		Assert.That(aggregate.Owner.Id, Is.EqualTo(todoItem.Owner!.Id));
		Assert.That(aggregate.Category, Is.Not.Null);
		Assert.That(aggregate.Category.Id, Is.EqualTo(todoItem.Category!.Id));
#pragma warning restore NUnit2045 // Use Assert.Multiple
	}

	[Test]
	public async Task GetPageAsync_IntegratesWithTodoItemsSpecification()
	{
#pragma warning disable NUnit2045 // Use Assert.Multiple
		// Arrange
		var todoItem = await CreateItemWithOwnerAndCategoryAsync();
		TodoItemsFilter filter = new(todoItem.Name, todoItem.OwnerId, [todoItem.State], [todoItem.CategoryId]);
		TodoItemsSpecification specification = new(todoItem.TodoListId, filter);

		// Act
		var page = await Repository.GetPageAsync<TodoItemPreviewDto>(new(1, 5), specification);

		// Assert
		var dto = page.Items.Where(x => x.Id == todoItem.Id).SingleOrDefault();
		Assert.That(dto, Is.Not.Null);
		Assert.That(dto.Category, Is.Not.Null);
		Assert.That(dto.Category.Id, Is.EqualTo(todoItem.CategoryId));
		Assert.That(dto.Owner, Is.Not.Null);
		Assert.That(dto.Owner.Id, Is.EqualTo(todoItem.OwnerId));
#pragma warning restore NUnit2045 // Use Assert.Multiple
	}
}
