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

	private readonly TodoListContext TestContext = new("TodoListId", "TestUserId");

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoItemsService>()!;
	}

	[Test]
	public async Task GetItemsOfListAsync_ListExists_AppliesTodoItemsSpecification()
	{
		// Arrange
		PaginationParameters parameters = new(2, 5);
		Page<TodoItemPreviewDto> page = new([], 1, 1, 1);
		WebApplicationFactory.TodoItemsHelperService
			.GetPageAsync<TodoItemPreviewDto>(TestContext, Arg.Any<ISpecification<TodoItem>>(), Arg.Any<PaginationParameters>())
			.Returns(new ServiceResponse<Page<TodoItemPreviewDto>>(ServiceResponseStatus.Success, page));

		// Act
		var result = await _service.GetItemsOfListAsync(TestContext, parameters);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoItemsHelperService
			.Received()
			.GetPageAsync<TodoItemPreviewDto>(TestContext,
			Arg.Is<TodoListDependantEntitiesSpecification<TodoItem>>(x => x.TodoListId == TestContext.TodoListId),
			Arg.Any<PaginationParameters>());
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_AppliesTodoListAggregateSpecification()
	{
		// Arrange
		int todoItemId = 123;
		TodoItemGetByIdDto dto = new(todoItemId, TestContext.TodoListId, "Name", "Description", null,
			TodoItemState.Active, new("User", "Name"));
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync(TestContext, todoItemId);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Result, Is.EqualTo(dto));
			Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Success));
		});
		await WebApplicationFactory.TodoItemsRepository
			.Received()
			.GetAggregateAsync<TodoItemGetByIdDto>(
			Arg.Is<TodoItemAggregateSpecification>(x => x.Id == todoItemId));
	}

	[Test]
	public async Task GetByIdAsync_InvalidTodoListId_ReturnsNotFound()
	{
		// Arrange
		int todoItemId = 123;
		TodoItemGetByIdDto dto = new(todoItemId, "WrongTodoListId", "Name", "Description", null,
			TodoItemState.Active, new("User", "Name"));
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync(TestContext, todoItemId);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Arrange
		int itemId = 123;
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync(TestContext, itemId);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task GetByIdAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		int itemId = 123;
		TodoItemGetByIdDto dto = new(itemId, TestContext.TodoListId, "Name", "Description", null,
			TodoItemState.Active, new("User", "Name"));
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(false);
		WebApplicationFactory.TodoItemsRepository
			.GetAggregateAsync<TodoItemGetByIdDto>(Arg.Any<ISpecification<TodoItem>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync(TestContext, itemId);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	// Tests for other methods are useless, because they are just wrappers.
}
