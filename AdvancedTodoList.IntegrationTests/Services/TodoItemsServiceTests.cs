using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;

namespace AdvancedTodoList.IntegrationTests.Services;

public class TodoItemsServiceTests : BusinessLogicFixture
{
	private ITodoItemsService _service;

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoItemsService>()!;
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_AppliesTodoListAggregateSpecification()
	{
		// Arrange
		int todoItemId = 123;
		string todoListId = "TodoListId";
		TodoItemGetByIdDto dto = new(todoItemId, todoListId, "Name", "Description", null,
			TodoItemState.Active, new("User", "Name"));
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync(todoListId, todoItemId);

		// Assert that valid specification was applied
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.GetAggregateAsync<TodoItemGetByIdDto>(
			Arg.Is<TodoItemAggregateSpecification>(x => x.Id == todoItemId));
	}

	[Test]
	public async Task GetByIdAsync_InvalidTodoListId_ReturnsNull()
	{
		// Arrange
		int todoItemId = 123;
		string todoListId = "TodoListId";
		TodoItemGetByIdDto dto = new(todoItemId, todoListId, "Name", "Description", null,
			TodoItemState.Active, new("User", "Name"));
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync("Wrong list ID", todoItemId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		int itemId = 123;
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync(todoListId, itemId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetItemsOfListAsync_ListExists_AppliesTodoItemsSpecification()
	{
		// Arrange
		string todoListId = "ID";
		PaginationParameters parameters = new(2, 5);
		WebApplicationFactory.TodoItemsHelperService
			.GetPageAsync<TodoItemPreviewDto>(todoListId, Arg.Any<ISpecification<TodoItem>>(), Arg.Any<PaginationParameters>())
			.Returns(new Page<TodoItemPreviewDto>([], 1, 1, 1));

		// Act
		var result = await _service.GetItemsOfListAsync(todoListId, parameters);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoItemsHelperService
			.Received()
			.GetPageAsync<TodoItemPreviewDto>(todoListId,
			Arg.Is<TodoListDependantEntitiesSpecification<TodoItem>>(x => x.TodoListId == todoListId),
			Arg.Any<PaginationParameters>());
	}

	[Test]
	public async Task CreateAsync_TodoListExists_AddsEntityToDb()
	{
		// Arrange: initialize a DTO
		string todoListId = "ID";
		string callerId = "CallerId";
		TodoItemCreateDto dto = new("Name", "Description", DateTime.UtcNow);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.AddAsync(Arg.Any<TodoItem>())
			.Returns(Task.FromResult);

		// Act: call the method
		var result = await _service.CreateAsync(todoListId, dto, callerId);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.AddAsync(Arg.Is<TodoItem>(x =>
			x.Name == dto.Name && x.Description == dto.Description && x.OwnerId == callerId));
	}

	[Test]
	public async Task CreateAsync_TodoListDoesNotExist_ReturnsNull()
	{
		// Arrange: initialize a DTO
		string todoListId = "ID";
		string callerId = "CallerId";
		TodoItemCreateDto dto = new("Name", "Description", DateTime.UtcNow);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act: call the method
		var result = await _service.CreateAsync(todoListId, dto, callerId);

		// Assert
		Assert.That(result, Is.Null);
	}

	// Tests for other methods are useless, because they are just wrappers.
}
