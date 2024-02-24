using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.IntegrationTests.Fixtures;
using NSubstitute.ReturnsExtensions;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

/// <summary>
/// Tests for endpoints 'api/todo/{listId}/items'
/// </summary>
[TestFixture]
public class TodoItemsEndpointsTests : EndpointsFixture
{
	[Test]
	public async Task GetListItemsAsync_ListExists_SucceedsAndReturnsItems()
	{
		// Arrange
		string testListId = "TestId";
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		TodoItemPreviewDto[] items =
		[
			new(124013, testListId, "1", null, TodoItemState.Active),
			new(124124, testListId, "2", null, TodoItemState.Completed)
		];
		WebApplicationFactory.TodoItemsService
			.GetItemsOfListAsync(testListId, parameters)
			.Returns(x => new(items, ((PaginationParameters)x[1]).Page,
			((PaginationParameters)x[1]).PageSize, 22));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items?page={parameters.Page}&pageSize={parameters.PageSize}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid page was returned
		var returnedPage = await result.Content.ReadFromJsonAsync<Page<TodoItemPreviewDto>>();
		Assert.That(returnedPage, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(returnedPage.PageNumber, Is.EqualTo(parameters.Page));
			Assert.That(returnedPage.PageSize, Is.EqualTo(parameters.PageSize));
			Assert.That(returnedPage.Items, Is.EquivalentTo(items));
		});
	}

	[Test]
	public async Task GetListItemsAsync_WrongPaginationParams_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetListItemsAsync_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetListItemsAsync_ListDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";

		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(testListId)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoItemById_ElementExists_ReturnsElement()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 777;
		TodoItemGetByIdDto testDto = new(testItemId, testListId, "Test todo item", "...", null, TodoItemState.Active);

		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(testListId, testItemId)
			.Returns(testDto);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid object was returned
		var returnedDto = await result.Content.ReadFromJsonAsync<TodoItemGetByIdDto>();
		Assert.That(returnedDto, Is.EqualTo(testDto));
	}

	[Test]
	public async Task GetTodoItemById_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 777;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoItemById_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 777;
		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(testListId, testItemId)
			.ReturnsNull();
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoItem_ValidCall_Succeeds()
	{
		// Arrange
		string listId = "ListId";
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(listId, dto)
			.Returns(new TodoItemGetByIdDto(500, "TodoListId", "", "", DateTime.UtcNow, TodoItemState.Active));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/items", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.CreateAsync(listId, dto);
	}

	[Test]
	public async Task PostTodoItem_TodoListDoesNotExist_Returns404()
	{
		// Arrange
		string listId = "ListId";
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(listId, dto)
			.ReturnsNull();
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/items", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoItem_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string listId = "ListId";
		TodoItemCreateDto dto = new("Name", "Desc", DateTime.UtcNow.AddDays(5));
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/items", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PostTodoItem_InvalidDto_Returns400()
	{
		// Arrange
		string listId = "ListId";
		TodoItemCreateDto invalidDto = new(string.Empty, string.Empty, DateTime.MinValue);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/items", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItem_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemCreateDto dto = new("Do nothing for entire day", "...", DateTime.MaxValue);
		WebApplicationFactory.TodoItemsService
			.EditAsync(testListId, testItemId, dto)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.EditAsync(testListId, testItemId, dto);
	}

	[Test]
	public async Task PutTodoItem_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 12412;
		TodoItemCreateDto dto = new("New name", "New description", null);
		WebApplicationFactory.TodoItemsService
			.EditAsync(testListId, testItemId, dto)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItem_InvalidDto_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemCreateDto invalidDto = new(string.Empty, string.Empty, DateTime.MinValue);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}
	[Test]
	public async Task PutTodoItem_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemCreateDto dto = new("Name", "Desc", DateTime.UtcNow.AddDays(5));
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PutTodoItemState_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(testListId, testItemId, dto)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.UpdateStateAsync(testListId, testItemId, dto);
	}

	[Test]
	public async Task PutTodoItemState_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 12412;
		TodoItemUpdateStateDto dto = new(TodoItemState.Skipped);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(testListId, testItemId, dto)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItemState_InvalidDto_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new((TodoItemState)213);

		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItemState_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new(TodoItemState.Active);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task DeleteTodoItem_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 504030;
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(testListId, testItemId)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.DeleteAsync(testListId, testItemId);
	}

	[Test]
	public async Task DeleteTodoItem_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 504030;
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(testListId, testItemId)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoItem_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
