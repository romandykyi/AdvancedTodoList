using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Infrastructure.Specifications;
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

	[Test]
	public async Task GetAggregateAsync_TodoListAggregateSpecification_IncludesOwner()
	{
		// Arrange
		var owner = TestModels.CreateTestUser();
		DbContext.Add(owner);
		await DbContext.SaveChangesAsync();
		var todoList = TestModels.CreateTestTodoList(owner.Id);
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		TodoListAggregateSpecification specification = new(todoList.Id);

		// Act
		var aggregate = await Repository.GetAggregateAsync<TodoListGetByIdDto>(specification);

		// Assert
		Assert.That(aggregate, Is.Not.Null);
		Assert.That(aggregate.Owner, Is.Not.Null);
		Assert.That(aggregate.Owner.Id, Is.EqualTo(owner.Id));
	}
}
