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

	// Tests for other methods are useless, because they are just wrappers.
}
