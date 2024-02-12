using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListsServiceTests : IntegrationTest
{
	private ITodoListsService _service;

	// Create a test to-do list and adds it to the DB
	private async Task<TodoList> CreateTestListAsync()
	{
		TodoList testList = new()
		{
			Name = "Test list",
			Description = ""
		};
		DbContext.TodoLists.Add(testList);
		await DbContext.SaveChangesAsync();
		DbContext.ChangeTracker.Clear();

		return testList;
	}

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoListsService>()!;
	}

	[Test]
	public async Task GetItemsOfListAsync_ListExists_ReturnsEmptyCollection()
	{
		// Arrange
		var list = await CreateTestListAsync();
		var fakeList = await CreateTestListAsync();
		TodoItem[] items = 
		[
			new TodoItem() { Name = "A", Description = "1", DeadlineDate = null, State = TodoItemState.Active, TodoListId = list.Id },
			new TodoItem() { Name = "B", Description = "2", DeadlineDate = DateTime.UtcNow, State = TodoItemState.Skipped, TodoListId = list.Id },
			new TodoItem() { Name = "C", Description = "3", DeadlineDate = null, State = TodoItemState.Completed, TodoListId = list.Id },
		];
		DbContext.TodoItems.AddRange(items);
		// Add one "fake" item
		TodoItem fakeItem = new() { Name = "Fake", Description = "", DeadlineDate = null, State = TodoItemState.Active, TodoListId = fakeList.Id };
		await DbContext.SaveChangesAsync();

		// Act
		var result = await _service.GetItemsOfListAsync(list.Id);

		// Assert
		Assert.That(result.Select(x => x.Id), Is.EquivalentTo(items.Select(x => x.Id)));
	}

	[Test]
	public async Task GetItemsOfListAsync_WrongId_ReturnsEmptyCollection()
	{
		// Act
		var result = await _service.GetItemsOfListAsync("_");

		// Assert
		Assert.That(result, Is.Empty);
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_ReturnsCorrectEntity()
	{
		// Arrange: add test list to the DB
		var testList = await CreateTestListAsync();

		// Act: try to obtain a test list's DTO by its ID
		var result = await _service.GetByIdAsync(testList.Id);

		// Assert that returned DTO matches
		Assert.That(result, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(result.Id, Is.EqualTo(testList.Id));
			Assert.That(result.Name, Is.EqualTo(testList.Name));
		});
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Act: try to obtain a test list with non-existent ID
		var result = await _service.GetByIdAsync("_");

		// Assert that null is returned
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_AddsEntityToDb()
	{
		// Arrange: initialize a DTO
		TodoListCreateDto dto = new("Test entity", "...");

		// Act: call the method
		var result = await _service.CreateAsync(dto);

		// Assert that entity was added to the DB
		var foundEntity = await DbContext.TodoLists
			.AsNoTracking()
			.Where(x => x.Id == result.Id)
			.SingleOrDefaultAsync();
		Assert.That(foundEntity, Is.Not.Null);
	}

	[Test]
	public async Task EditAsync_EntityExists_Succeeds()
	{
		// Arrange: add test list to the DB
		var testList = await CreateTestListAsync();
		// Test edit DTO
		TodoListCreateDto dto = new("Edited name", "Edited description");

		// Act: edit the list
		bool result = await _service.EditAsync(testList.Id, dto);

		// Assert that result is true
		Assert.That(result);
		// Assert that list was updated
		var updatedList = await DbContext.TodoLists
			.AsNoTracking()
			.Where(x => x.Id == testList.Id)
			.FirstAsync();
		Assert.Multiple(() =>
		{
			Assert.That(updatedList.Name, Is.EqualTo(dto.Name));
			Assert.That(updatedList.Description, Is.EqualTo(dto.Description));
		});
	}

	[Test]
	public async Task EditAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange: make a DTO
		TodoListCreateDto dto = new("This", "should not be used if code works properly.");

		// Act: try to edit a non-existent list
		bool result = await _service.EditAsync("_", dto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_EntityExists_Succeeds()
	{
		// Arrange: add test list to the DB
		var testList = await CreateTestListAsync();

		// Act: delete the list
		bool result = await _service.DeleteAsync(testList.Id);

		// Assert that result is true
		Assert.That(result);
		// Assert that list was deleted
		var actualList = await DbContext.TodoLists
			.AsNoTracking()
			.Where(x => x.Id == testList.Id)
			.FirstOrDefaultAsync();
		Assert.That(actualList, Is.Null);
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Act: try to delete a non-existent list
		bool result = await _service.DeleteAsync("_");

		// Assert that result is false
		Assert.That(result, Is.False);
	}
}
