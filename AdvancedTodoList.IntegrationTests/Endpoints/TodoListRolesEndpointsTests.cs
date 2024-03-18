using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoListRolesEndpointsTests : EndpointsFixture
{
	private readonly TodoListContext TestContext = new("TodoListId", TestUserId);

	[Test]
	public async Task GetTodoListRolesAsync_ValidCall_SucceedsAndReturnsItems()
	{
		// Arrange
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		TodoListRolePreviewDto[] roles =
		[
			new(700, "Admin"),
			new(701, "Reviewer"),
		];
		string name = "n";
		WebApplicationFactory.TodoListRolesService
			.GetRolesOfListAsync(TestContext, parameters, name)
			.Returns(x => new ServiceResponse<Page<TodoListRolePreviewDto>>(
				ServiceResponseStatus.Success, new(roles, ((PaginationParameters)x[1]).Page,
				((PaginationParameters)x[1]).PageSize, 22)));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles?page={parameters.Page}&pageSize={parameters.PageSize}&name={name}");

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
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetTodoListRolesAsync_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListRolesAsync_NotFoundStatus_Returns404()
	{
		// Arrange
		WebApplicationFactory.TodoListRolesService
			.GetRolesOfListAsync(TestContext, Arg.Any<PaginationParameters>())
			.Returns(x => new ServiceResponse<Page<TodoListRolePreviewDto>>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoListRolesAsync_ForbiddenStatus_Returns403()
	{
		// Arrange
		WebApplicationFactory.TodoListRolesService
			.GetRolesOfListAsync(TestContext, Arg.Any<PaginationParameters>())
			.Returns(x => new ServiceResponse<Page<TodoListRolePreviewDto>>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles?page=1&pageSize=20");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task GetTodoListRoleById_ValidCall_ReturnsElement()
	{
		// Arrange
		int testRoleId = 777;
		TodoListRoleViewDto testDto = new(testRoleId, "Name", 5, new(true));

		WebApplicationFactory.TodoListRolesService
			.GetByIdAsync(TestContext, testRoleId)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.Success, testDto));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

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
		int testRoleId = 777;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListRoleById_NotFoundStatus_Returns404()
	{
		// Arrange
		int testRoleId = 777;
		WebApplicationFactory.TodoListRolesService
			.GetByIdAsync(TestContext, testRoleId)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoListRoleById_ForbiddenStatus_Returns404()
	{
		// Arrange
		int testRoleId = 777;
		WebApplicationFactory.TodoListRolesService
			.GetByIdAsync(TestContext, testRoleId)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostTodoListRole_ValidCall_Succeeds()
	{
		// Arrange
		TodoListRoleCreateDto dto = new("Name", 5, new(true));
		WebApplicationFactory.TodoListRolesService
			.CreateAsync(TestContext, dto)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.Success));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoListRolesService
			.Received()
			.CreateAsync(TestContext, dto);
	}

	[Test]
	public async Task PostTodoListRole_NotFoundStatus_Returns404()
	{
		// Arrange
		TodoListRoleCreateDto dto = new("Joker", 5, new(true));
		WebApplicationFactory.TodoListRolesService
			.CreateAsync(TestContext, dto)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoListRole_ForbiddenStatus_Returns403()
	{
		// Arrange
		TodoListRoleCreateDto dto = new("Joker", 5, new(true));
		WebApplicationFactory.TodoListRolesService
			.CreateAsync(TestContext, dto)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostTodoListRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		TodoListRoleCreateDto dto = new("Joker", 5, new(true));
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PostTodoListRole_InvalidDto_Returns400()
	{
		// Arrange
		TodoListRoleCreateDto invalidDto = new(string.Empty, 5, new());
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoListRole_ValidCall_Succeeds()
	{
		// Arrange
		int testRoleId = 891349;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		WebApplicationFactory.TodoListRolesService
			.EditAsync(TestContext, testRoleId, dto)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoListRolesService
			.Received()
			.EditAsync(TestContext, testRoleId, dto);
	}

	[Test]
	public async Task PutTodoListRole_NotFoundStatus_Returns404()
	{
		// Arrange
		int testRoleId = 12412;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		WebApplicationFactory.TodoListRolesService
			.EditAsync(TestContext, testRoleId, dto)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoListRole_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testRoleId = 12412;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		WebApplicationFactory.TodoListRolesService
			.EditAsync(TestContext, testRoleId, dto)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PutTodoListRole_InvalidDto_Returns400()
	{
		// Arrange
		int testRoleId = 891349;
		TodoListRoleCreateDto invalidDto = new(string.Empty, 5, new());
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoListRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testRoleId = 891349;
		TodoListRoleCreateDto dto = new("New name", 5, new(true, true, true));
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task DeleteTodoListRole_ValidCall_Succeeds()
	{
		// Arrange
		int testRoleId = 504030;
		WebApplicationFactory.TodoListRolesService
			.DeleteAsync(TestContext, testRoleId)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoListRolesService
			.Received()
			.DeleteAsync(TestContext, testRoleId);
	}

	[Test]
	public async Task DeleteTodoListRole_NotFoundStatus_Returns404()
	{
		// Arrange
		int testRoleId = 504030;
		WebApplicationFactory.TodoListRolesService
			.DeleteAsync(TestContext, testRoleId)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoListRole_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testRoleId = 504030;
		WebApplicationFactory.TodoListRolesService
			.DeleteAsync(TestContext, testRoleId)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task DeleteTodoListRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testRoleId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/roles/{testRoleId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
