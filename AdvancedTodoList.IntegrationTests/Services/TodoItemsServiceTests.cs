using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using Mapster;
using NSubstitute.ReturnsExtensions;

namespace AdvancedTodoList.IntegrationTests.Services;

/// <summary>
/// Tests for endpoints 'api/todo'
/// </summary>
public class TodoItemsServiceTests : BusinessLogicFixture
{
	private ITodoItemsService _service;

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoItemsService>()!;
	}

	[Test]
	public async Task GetItemsOfListAsync_ListExists_AppliesTodoItemsSpecification()
	{
		// Arrange
		string todoListId = "ID";
		PaginationParameters parameters = new(2, 5);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetPageAsync<TodoItemPreviewDto>(Arg.Any<PaginationParameters>(), Arg.Any<ISpecification<TodoItem>>())
			.Returns(new Page<TodoItemPreviewDto>([], parameters.Page, parameters.PageSize, 0));

		// Act
		var result = await _service.GetItemsOfListAsync(todoListId, parameters);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.GetPageAsync<TodoItemPreviewDto>(parameters, Arg.Any<TodoItemsSpecification>());
	}

	[Test]
	public async Task GetItemsOfListAsync_ListDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		PaginationParameters parameters = new(2, 5);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		var result = await _service.GetItemsOfListAsync(todoListId, parameters);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_ReturnsCorrectEntity()
	{
		// Arrange
		string todoListId = "ID";
		TodoItem todoItem = TestModels.CreateTestTodoItem(todoListId);
		todoItem.Id = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItem.Id)
			.Returns(todoItem);

		// Act
		var result = await _service.GetByIdAsync(todoListId, todoItem.Id);

		// Assert that returned DTO matches
		var expectedResult = todoItem.Adapt<TodoItemGetByIdDto>();
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task GetByIdAsync_TodoListDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		var result = await _service.GetByIdAsync(todoListId, todoItemId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItemId)
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync(todoListId, todoItemId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_TodoListExists_AddsEntityToDb()
	{
		// Arrange: initialize a DTO
		string todoListId = "ID";
		TodoItemCreateDto dto = new("Test entity", "...", DateTime.UtcNow.AddDays(300));
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.AddAsync(Arg.Any<TodoItem>())
			.Returns(Task.FromResult);

		// Act: call the method
		var result = await _service.CreateAsync(todoListId, dto);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.AddAsync(Arg.Is<TodoItem>(x => x.Name == dto.Name &&
			x.Description == dto.Description && x.TodoListId == todoListId));
	}

	[Test]
	public async Task CreateAsync_TodoListDoesNotExist_ReturnsNull()
	{
		// Arrange: initialize a DTO
		string todoListId = "ID";
		TodoItemCreateDto dto = new("Test entity", "...", DateTime.UtcNow.AddDays(300));
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act: call the method
		var result = await _service.CreateAsync(todoListId, dto);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task EditAsync_EntityExists_Succeeds()
	{
		// Arrange
		string todoListId = "ID";
		var todoItem = TestModels.CreateTestTodoItem(todoListId);
		TodoItemCreateDto updateDto = new("NewName", "NewDescription", DateTime.UtcNow.AddDays(30));
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItem.Id)
			.Returns(todoItem);
		WebApplicationFactory.TodoItemsRepository
			.UpdateAsync(Arg.Any<TodoItem>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.EditAsync(todoListId, todoItem.Id, updateDto);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that update was called
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.UpdateAsync(Arg.Is<TodoItem>(x => x.Id == todoItem.Id &&
				x.Name == updateDto.Name &&
				x.Description == updateDto.Description));
	}

	[Test]
	public async Task EditAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		TodoItemCreateDto updateDto = new("NewName", "NewDescription", DateTime.UtcNow.AddDays(30));
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItemId)
			.ReturnsNull();

		// Act
		bool result = await _service.EditAsync(todoListId, todoItemId, updateDto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task EditAsync_TodoListDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		TodoItemCreateDto updateDto = new("NewName", "NewDescription", DateTime.UtcNow.AddDays(30));
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		bool result = await _service.EditAsync(todoListId, todoItemId, updateDto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task UpdateStateAsync_EntityExists_Succeeds()
	{
		// Arrange
		string todoListId = "ID";
		var todoItem = TestModels.CreateTestTodoItem(todoListId);
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItem.Id)
			.Returns(todoItem);
		WebApplicationFactory.TodoItemsRepository
			.UpdateAsync(Arg.Any<TodoItem>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.UpdateStateAsync(todoListId, todoItem.Id, dto);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that update was called
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.UpdateAsync(Arg.Is<TodoItem>(x => x.Id == todoItem.Id &&
				x.State == dto.State));
	}

	[Test]
	public async Task UpdateStateAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItemId)
			.ReturnsNull();

		// Act
		bool result = await _service.UpdateStateAsync(todoListId, todoItemId, dto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task UpdateStateAsync_TodoListDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		bool result = await _service.UpdateStateAsync(todoListId, todoItemId, dto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_EntityExists_Succeeds()
	{
		// Arrange
		string todoListId = "ID";
		var todoItem = TestModels.CreateTestTodoItem(todoListId);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItem.Id)
			.Returns(todoItem);
		WebApplicationFactory.TodoItemsRepository
			.DeleteAsync(Arg.Any<TodoItem>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.DeleteAsync(todoListId, todoItem.Id);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that delete was called
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.DeleteAsync(Arg.Is<TodoItem>(x => x.Id == todoItem.Id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetByIdAsync(todoItemId)
			.ReturnsNull();
		WebApplicationFactory.TodoItemsRepository
			.DeleteAsync(Arg.Any<TodoItem>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.DeleteAsync(todoListId, todoItemId);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_TodoListDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int todoItemId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		bool result = await _service.DeleteAsync(todoListId, todoItemId);

		// Assert that result is false
		Assert.That(result, Is.False);
	}
}
