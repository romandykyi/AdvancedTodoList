using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.RouteTests;
using NSubstitute.ReturnsExtensions;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoItem.RouteTests.Tests;

/// <summary>
/// Tests for endpoints 'api/todo/{listId}/items'
/// </summary>
public class TodoItemsEndpointsTests : RouteTest
{
	[Test]
	public async Task GetTodoItems_ListExists_SucceedsAndReturnsItems()
	{
		// Arrange
		string testListId = "TestId";
		TodoItemPreviewDto[] collection =
		[
			new(124013, testListId, "1", null, TodoItemState.Active),
			new(124124, testListId, "2", null, TodoItemState.Completed)
		];

		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(testListId)
			.Returns(true);
		WebApplicationFactory.TodoItemsService
			.GetItemsOfListAsync(testListId)
			.Returns(collection);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid collection was returned
		var returnedCollection = await result.Content.ReadFromJsonAsync<TodoItemPreviewDto[]>();
		Assert.That(returnedCollection, Is.Not.Null);
		Assert.That(returnedCollection, Is.EquivalentTo(collection));
	}

	[Test]
	public async Task GetTodoItems_ListDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";

		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(testListId)
			.Returns(false);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/");

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
			.GetByIdAsync(testItemId)
			.Returns(testDto);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid object was returned
		var returnedDto = await result.Content.ReadFromJsonAsync<TodoItemGetByIdDto>();
		Assert.That(returnedDto, Is.Not.Null);
		Assert.That(returnedDto, Is.EqualTo(testDto));
	}

	[Test]
	public async Task GetTodoItemById_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 777;
		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(testItemId)
			.ReturnsNull();
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoItemById_WrongTodoListId_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 777;
		TodoItemGetByIdDto testDto = new(testItemId, "RealListId", "Test todo item", "...", null, TodoItemState.Active);
		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(testItemId)
			.Returns(testDto);
		using HttpClient client = WebApplicationFactory.CreateClient();

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
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(listId)
			.Returns(true);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(Arg.Any<string>(), Arg.Any<TodoItemCreateDto>())
			.Returns(new TodoItem() { Id = 13480, Name = "A", Description = "", DeadlineDate = DateTime.UtcNow });
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue);
		using HttpClient client = WebApplicationFactory.CreateClient();

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
	public async Task PostTodoItem_InvalidDto_Returns400()
	{
		// Arrange
		string listId = "ListId";
		TodoItemCreateDto invalidDto = new(string.Empty, string.Empty, DateTime.MinValue);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/items", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PostTodoItem_TodoListDoesNotExist_Returns404()
	{
		// Arrange
		string listId = "ListId";
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(listId)
			.Returns(false);
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/items", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItem_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemCreateDto dto = new("Do nothing for entire day", "...", DateTime.MaxValue);

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns(testListId);
		WebApplicationFactory.TodoItemsService
			.EditAsync(testItemId, dto)
			.Returns(true);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.EditAsync(testItemId, dto);
	}

	[Test]
	public async Task PutTodoItem_InvalidDto_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemCreateDto invalidDto = new(string.Empty, string.Empty, DateTime.MinValue);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItem_WrongTodoListId_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemCreateDto dto = new("Do nothing for entire day", "...", DateTime.MaxValue);

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns("WrongId");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItem_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 12412;
		TodoItemCreateDto dto = new("New name", "New description", null);

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns(testListId);
		WebApplicationFactory.TodoItemsService
			.EditAsync(testItemId, dto)
			.Returns(false);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItemState_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns(testListId);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(testItemId, dto)
			.Returns(true);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.UpdateStateAsync(testItemId, dto);
	}

	[Test]
	public async Task PutTodoItemState_InvalidDto_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new((TodoItemState)213);

		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItemState_WrongTodoListId_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new(TodoItemState.Skipped);

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns("WrongId");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItemState_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 12412;
		TodoItemUpdateStateDto dto = new(TodoItemState.Skipped);

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns(testListId);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(testItemId, dto)
			.Returns(false);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/items/{testItemId}/state", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoItem_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 504030;

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns(testListId);
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(testItemId)
			.Returns(true);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.DeleteAsync(testItemId);
	}

	[Test]
	public async Task DeleteTodoItem_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 504030;

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns(testListId);
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(testItemId)
			.Returns(false);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoItem_WrongTodoListId_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testItemId = 504030;

		WebApplicationFactory.TodoItemsService
			.GetTodoListByIdAsync(testItemId)
			.Returns("WrongId");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}
}
