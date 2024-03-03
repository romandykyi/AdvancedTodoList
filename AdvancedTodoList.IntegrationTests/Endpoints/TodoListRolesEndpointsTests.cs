using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoListRolesEndpointsTests : EndpointsFixture
{
	[Test]
	public async Task GetTodoListRolesAsync_ListExists_SucceedsAndReturnsItems()
	{
		// Arrange
		string testListId = "TestId";
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		TodoListRolePreviewDto[] roles =
		[
			new(700, "Admin"),
			new(701, "Reviewer"),
		];
		WebApplicationFactory.TodoListRolesService
			.GetRolesOfListAsync(testListId, parameters)
			.Returns(x => new(roles, ((PaginationParameters)x[1]).Page,
			((PaginationParameters)x[1]).PageSize, 22));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles?page={parameters.Page}&pageSize={parameters.PageSize}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid page was returned
		var returnedPage = await result.Content.ReadFromJsonAsync<Page<TodoListRolePreviewDto>>();
		Assert.That(returnedPage, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(returnedPage.PageNumber, Is.EqualTo(parameters.Page));
			Assert.That(returnedPage.PageSize, Is.EqualTo(parameters.PageSize));
			Assert.That(returnedPage.Items, Is.EquivalentTo(roles));
		});
	}

	[Test]
	public async Task GetTodoListRolesAsync_WrongPaginationParams_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetTodoListRolesAsync_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListRolesAsync_ListDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";

		WebApplicationFactory.TodoListRolesService
			.GetRolesOfListAsync(testListId, Arg.Any<PaginationParameters>())
			.ReturnsNull();
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoListRoleById_ElementExists_ReturnsElement()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 777;
		TodoListRoleViewDto testDto = new(testRoleId, "Name", 5, new(true));

		WebApplicationFactory.TodoListRolesService
			.GetByIdAsync(testListId, testRoleId)
			.Returns(testDto);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles/{testRoleId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid object was returned
		var returnedDto = await result.Content.ReadFromJsonAsync<TodoListRoleViewDto>();
		Assert.That(returnedDto, Is.EqualTo(testDto));
	}

	[Test]
	public async Task GetTodoListRoleById_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 777;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles/{testRoleId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListRoleById_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 777;
		WebApplicationFactory.TodoListRolesService
			.GetByIdAsync(testListId, testRoleId)
			.ReturnsNull();
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/roles/{testRoleId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoListRole_ValidCall_Succeeds()
	{
		// Arrange
		string listId = "ListId";
		TodoListRoleCreateDto dto = new("Name", 5, new(true));
		WebApplicationFactory.TodoListRolesService
			.CreateAsync(listId, dto)
			.Returns(new TodoListRoleViewDto(777, dto.Name, 5, dto.Permissions));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/roles", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoListRolesService
			.Received()
			.CreateAsync(listId, dto);
	}

	[Test]
	public async Task PostTodoListRole_TodoListDoesNotExist_Returns404()
	{
		// Arrange
		string listId = "ListId";
		TodoListRoleCreateDto dto = new("Joker", 5, new(true));
		WebApplicationFactory.TodoListRolesService
			.CreateAsync(listId, dto)
			.ReturnsNull();
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/roles", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoListRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string listId = "ListId";
		TodoListRoleCreateDto dto = new("Joker", 5, new(true));
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/roles", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PostTodoListRole_InvalidDto_Returns400()
	{
		// Arrange
		string listId = "ListId";
		TodoListRoleCreateDto invalidDto = new(string.Empty, 5, new());
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/roles", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoListRole_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 891349;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		WebApplicationFactory.TodoListRolesService
			.EditAsync(testListId, testRoleId, dto)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/roles/{testRoleId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoListRolesService
			.Received()
			.EditAsync(testListId, testRoleId, dto);
	}

	[Test]
	public async Task PutTodoListRole_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 12412;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		WebApplicationFactory.TodoListRolesService
			.EditAsync(testListId, testRoleId, dto)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/roles/{testRoleId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoListRole_InvalidDto_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 891349;
		TodoListRoleCreateDto invalidDto = new(string.Empty, 5, new());
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/roles/{testRoleId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}
	[Test]
	public async Task PutTodoListRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 891349;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/roles/{testRoleId}", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task DeleteTodoListRole_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 504030;
		WebApplicationFactory.TodoListRolesService
			.DeleteAsync(testListId, testRoleId)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/roles/{testRoleId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoListRolesService
			.Received()
			.DeleteAsync(testListId, testRoleId);
	}

	[Test]
	public async Task DeleteTodoListRole_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 504030;
		WebApplicationFactory.TodoListRolesService
			.DeleteAsync(testListId, testRoleId)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/roles/{testRoleId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoListRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testRoleId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/roles/{testRoleId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
