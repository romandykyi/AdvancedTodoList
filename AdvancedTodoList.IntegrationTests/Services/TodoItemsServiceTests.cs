﻿using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedTodoList.IntegrationTests.Services;

/// <summary>
/// Tests for endpoints 'api/todo'
/// </summary>
public class TodoItemsServiceTests : DataAccessFixture
{
	private ITodoItemsService _service;

	// Create a test to-do list and adds it to the DB
	private async Task<TodoList> CreateTestListAsync()
	{
		TodoList testList = new()
		{
			Name = "Test list",
			Description = "A test list for creating a test list item"
		};
		DbContext.TodoLists.Add(testList);
		await DbContext.SaveChangesAsync();
		DbContext.ChangeTracker.Clear();

		return testList;
	}

	// Create a test to-do list item and adds it to the DB
	private async Task<TodoItem> CreateTestItemAsync()
	{
		TodoList testList = await CreateTestListAsync();

		TodoItem testItem = new()
		{
			Name = "Test list item",
			Description = "Make something cool",
			DeadlineDate = null,
			State = TodoItemState.Active,
			TodoListId = testList.Id
		};
		DbContext.TodoItems.Add(testItem);
		await DbContext.SaveChangesAsync();

		DbContext.ChangeTracker.Clear();

		return testItem;
	}

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoItemsService>()!;
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
	public async Task GetTodoListByIdAsync_EntityExists_ReturnsCorrectEntity()
	{
		// Arrange: add test item to the DB
		var testItem = await CreateTestItemAsync();

		// Act: try to obtain a test item's DTO by its ID
		string? result = await _service.GetTodoListByIdAsync(testItem.Id);

		// Assert that returned ID matches
		Assert.That(result, Is.EqualTo(testItem.TodoListId));
	}

	[Test]
	public async Task GetTodoListByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Act: try to obtain a test item with non-existent ID
		var result = await _service.GetTodoListByIdAsync(-1);

		// Assert that null is returned
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_ReturnsCorrectEntity()
	{
		// Arrange: add test item to the DB
		var testItem = await CreateTestItemAsync();

		// Act: try to obtain a test item's DTO by its ID
		var result = await _service.GetByIdAsync(testItem.Id);

		// Assert that returned DTO matches
		Assert.That(result, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(result.Id, Is.EqualTo(testItem.Id));
			Assert.That(result.Name, Is.EqualTo(testItem.Name));
		});
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Act: try to obtain a test item with non-existent ID
		var result = await _service.GetByIdAsync(-1);

		// Assert that null is returned
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_AddsEntityToDb()
	{
		// Arrange
		var list = await CreateTestListAsync();
		TodoItemCreateDto dto = new("Test entity", "...", null);

		// Act: call the method
		var result = await _service.CreateAsync(list.Id, dto);

		// Assert that entity was added to the DB
		var foundEntity = await DbContext.TodoItems
			.AsNoTracking()
			.Where(x => x.Id == result.Id)
			.SingleOrDefaultAsync();
		Assert.That(foundEntity, Is.Not.Null);
		Assert.That(foundEntity.TodoListId, Is.EqualTo(list.Id));
	}

	[Test]
	public async Task EditAsync_EntityExists_Succeeds()
	{
		// Arrange: add test item to the DB
		var testItem = await CreateTestItemAsync();
		// Test edit DTO
		TodoItemCreateDto dto = new("Edited name", "Edited description", DateTime.UtcNow);

		// Act: edit the item
		bool result = await _service.EditAsync(testItem.Id, dto);

		// Assert that result is true
		Assert.That(result);
		// Assert that item was updated
		var updatedItem = await DbContext.TodoItems
			.AsNoTracking()
			.Where(x => x.Id == testItem.Id)
			.FirstAsync();
		Assert.Multiple(() =>
		{
			Assert.That(updatedItem.Name, Is.EqualTo(dto.Name));
			Assert.That(updatedItem.Description, Is.EqualTo(dto.Description));
			Assert.That(updatedItem.DeadlineDate, Is.EqualTo(dto.DeadlineDate));
		});
	}

	[Test]
	public async Task EditAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange: make a DTO
		TodoItemCreateDto dto = new("This", "should not be used if code works properly.", null);

		// Act: try to edit a non-existent item
		bool result = await _service.EditAsync(-1, dto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task UpdateStateAsync_EntityExists_Succeeds()
	{
		// Arrange: add test item to the DB
		var testItem = await CreateTestItemAsync();
		// Test DTO
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);

		// Act: update the state of the item
		bool result = await _service.UpdateStateAsync(testItem.Id, dto);

		// Assert that result is true
		Assert.That(result);
		// Assert that item's state was updated
		var updatedItem = await DbContext.TodoItems
			.AsNoTracking()
			.Where(x => x.Id == testItem.Id)
			.FirstAsync();
		Assert.That(updatedItem.State, Is.EqualTo(dto.State));
	}

	[Test]
	public async Task UpdateStateAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange: make a DTO
		TodoItemUpdateStateDto dto = new(TodoItemState.Skipped);

		// Act: try to update the state of a non-existent item
		bool result = await _service.UpdateStateAsync(-1, dto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_EntityExists_Succeeds()
	{
		// Arrange: add test item to the DB
		var testItem = await CreateTestItemAsync();

		// Act: delete the item
		bool result = await _service.DeleteAsync(testItem.Id);

		// Assert that result is true
		Assert.That(result);
		// Assert that an item was deleted
		var actualList = await DbContext.TodoItems
			.AsNoTracking()
			.Where(x => x.Id == testItem.Id)
			.FirstOrDefaultAsync();
		Assert.That(actualList, Is.Null);
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Act: try to delete a non-existent item
		bool result = await _service.DeleteAsync(-1);

		// Assert that result is false
		Assert.That(result, Is.False);
	}
}
