using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoListsEndpointsTests : EndpointsFixture
{
	[Test]
	public async Task GetTodoListById_ElementExists_ReturnsElement()
	{
		// Arrange
		string testId = "TestId";
		TodoListGetByIdDto testDto = new(testId, "Test todo list", "", new("Id", "User"));
		WebApplicationFactory.TodoListsService
			.GetByIdAsync(testId, TestUserId)
			.Returns(new ServiceResponse<TodoListGetByIdDto>(ServiceResponseStatus.Success, testDto));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid object was returned
		var returnedDto = await result.Content.ReadFromJsonAsync<TodoListGetByIdDto>();
		Assert.That(returnedDto, Is.EqualTo(testDto));
	}

	[Test]
	public async Task GetTodoListById_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testId = "TestId";
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListById_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testId = "TestId";
		WebApplicationFactory.TodoListsService
			.GetByIdAsync(testId, TestUserId)
			.Returns(new ServiceResponse<TodoListGetByIdDto>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoListById_UserHasNoPermission_Returns401()
	{
		// Arrange
		string testId = "TestId";
		WebApplicationFactory.TodoListsService
			.GetByIdAsync(testId, TestUserId)
			.Returns(new ServiceResponse<TodoListGetByIdDto>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostTodoList_ValidCall_Succeeds()
	{
		// Arrange
		WebApplicationFactory.TodoListsService
			.CreateAsync(Arg.Any<TodoListCreateDto>(), Arg.Any<string>())
			.Returns(new TodoListGetByIdDto("Id", "", "", new("Id", "User")));
		TodoListCreateDto dto = new("Test", string.Empty);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync("api/todo", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoListsService
			.Received()
			.CreateAsync(dto, TestUserId);
	}

	[Test]
	public async Task PostTodoList_InvalidDto_Returns400()
	{
		// Arrange
		TodoListCreateDto invalidDto = new(string.Empty, string.Empty);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync("api/todo", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PostTodoList_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		TodoListCreateDto dto = new("Name", "Descr");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync("api/todo", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PutTodoList_ElementExists_Succeeds()
	{
		// Arrange
		string testId = "TestId";
		TodoListCreateDto dto = new("New name", "New description");

		WebApplicationFactory.TodoListsService
			.EditAsync(testId, dto, TestUserId)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoListsService
			.Received()
			.EditAsync(testId, dto, TestUserId);
	}

	[Test]
	public async Task PutTodoList_InvalidDto_Returns400()
	{
		// Arrange
		string testId = "TestId";
		TodoListCreateDto invalidDto = new(string.Empty, string.Empty);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoList_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testId = "TestId";
		TodoListCreateDto invalidDto = new("Name", "Descr");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testId}", invalidDto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PutTodoList_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testId = "TestId";
		TodoListCreateDto dto = new("New name", "New description");

		WebApplicationFactory.TodoListsService
			.EditAsync(testId, dto, TestUserId)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoList_UserHasNoPermission_Returns403()
	{
		// Arrange
		string testId = "TestId";
		TodoListCreateDto dto = new("New name", "New description");

		WebApplicationFactory.TodoListsService
			.EditAsync(testId, dto, TestUserId)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task DeleteTodoList_ElementExists_Succeeds()
	{
		// Arrange
		string testId = "TestId";

		WebApplicationFactory.TodoListsService
			.DeleteAsync(testId, TestUserId)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoListsService
			.Received()
			.DeleteAsync(testId, TestUserId);
	}

	[Test]
	public async Task DeleteTodoList_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testId = "TestId";

		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task DeleteTodoList_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testId = "TestId";

		WebApplicationFactory.TodoListsService
			.DeleteAsync(testId, TestUserId)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoList_UserHasNoPermission_Returns401()
	{
		// Arrange
		string testId = "TestId";

		WebApplicationFactory.TodoListsService
			.DeleteAsync(testId, TestUserId)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}
}
