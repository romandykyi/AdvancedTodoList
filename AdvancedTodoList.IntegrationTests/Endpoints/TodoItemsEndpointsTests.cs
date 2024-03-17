using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

/// <summary>
/// Tests for endpoints 'api/todo/{TestContext.TodoListId}/items'
/// </summary>
[TestFixture]
public class TodoItemsEndpointsTests : EndpointsFixture
{
	private readonly TodoListContext TestContext = new("TodoListId", TestUserId);

	[Test]
	public async Task GetListItemsAsync_ValidCall_SucceedsAndReturnsItems()
	{
		// Arrange
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		TodoItemPreviewDto[] items =
		[
			new(124013, TestContext.TodoListId, "1", null, TodoItemState.Active, 3, null!, null),
			new(124124, TestContext.TodoListId, "2", null, TodoItemState.Completed, 3, null!, null)
		];
		WebApplicationFactory.TodoItemsService
			.GetItemsOfListAsync(TestContext, parameters, Arg.Any<TodoItemsFilter>())
			.Returns(x => new ServiceResponse<Page<TodoItemPreviewDto>>(
				ServiceResponseStatus.Success, new(items, ((PaginationParameters)x[1]).Page,
				((PaginationParameters)x[1]).PageSize, 22)));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items?page={parameters.Page}&pageSize={parameters.PageSize}");

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
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetListItemsAsync_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetListItemsAsync_NotFoundStatus_Returns404()
	{
		// Arrange
		WebApplicationFactory.TodoItemsService
			.GetItemsOfListAsync(TestContext, Arg.Any<PaginationParameters>(), Arg.Any<TodoItemsFilter>())
			.Returns(new ServiceResponse<Page<TodoItemPreviewDto>>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetListItemsAsync_ForbiddenStatus_Returns403()
	{
		// Arrange
		WebApplicationFactory.TodoItemsService
			.GetItemsOfListAsync(TestContext, Arg.Any<PaginationParameters>(), Arg.Any<TodoItemsFilter>())
			.Returns(new ServiceResponse<Page<TodoItemPreviewDto>>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items?page=1&pageSize=20");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task GetTodoItemById_ValidCall_ReturnsElement()
	{
		// Arrange
		int testItemId = 777;
		TodoItemGetByIdDto testDto = new(testItemId, TestContext.TodoListId, "Test todo item",
			"...", null, TodoItemState.Active, 5, new("Id", "User"), null);

		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(TestContext, testItemId)
			.Returns(new ServiceResponse<TodoItemGetByIdDto>(ServiceResponseStatus.Success, testDto));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

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
		int testItemId = 777;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoItemById_NotFoundStatus_Returns404()
	{
		// Arrange
		int testItemId = 777;
		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(TestContext, testItemId)
			.Returns(new ServiceResponse<TodoItemGetByIdDto>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoItemById_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testItemId = 777;
		WebApplicationFactory.TodoItemsService
			.GetByIdAsync(TestContext, testItemId)
			.Returns(new ServiceResponse<TodoItemGetByIdDto>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostTodoItem_ValidCall_Succeeds()
	{
		// Arrange
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue, 9, null);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(TestContext, dto)
			.Returns(new TodoItemsServiceResponse(TodoItemsServiceStatus.Success));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/items", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.CreateAsync(TestContext, dto);
	}

	[Test]
	public async Task PostTodoItem_NotFoundStatus_Returns404()
	{
		// Arrange
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue, 9, null);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(TestContext, dto)
			.Returns(new TodoItemsServiceResponse(TodoItemsServiceStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/items", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoItem_ForbiddenStatus_Returns403()
	{
		// Arrange
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue, 9, null);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(TestContext, dto)
			.Returns(new TodoItemsServiceResponse(TodoItemsServiceStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/items", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostTodoItem_InvalidCategoryIdStatus_Returns400()
	{
		// Arrange
		TodoItemCreateDto dto = new("Item", "...", DateTime.MaxValue, 9, null);
		WebApplicationFactory.TodoItemsService
			.CreateAsync(TestContext, dto)
			.Returns(new TodoItemsServiceResponse(TodoItemsServiceStatus.InvalidCategoryId));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/items", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PostTodoItem_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		TodoItemCreateDto dto = new("Name", "Desc", DateTime.UtcNow.AddDays(5), 9, null);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/items", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PostTodoItem_InvalidDto_Returns400()
	{
		// Arrange
		TodoItemCreateDto invalidDto = new(string.Empty, string.Empty, DateTime.MinValue, 9, null);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/items", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItem_ValidCall_Succeeds()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemCreateDto dto = new("Do nothing for entire day", "...", DateTime.MaxValue, 9, null);
		WebApplicationFactory.TodoItemsService
			.EditAsync(TestContext, testItemId, dto)
			.Returns(TodoItemsServiceStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.EditAsync(TestContext, testItemId, dto);
	}

	[Test]
	public async Task PutTodoItem_NotFoundStatus_Returns404()
	{
		// Arrange
		int testItemId = 12412;
		TodoItemCreateDto dto = new("New name", "New description", null, 5, null);
		WebApplicationFactory.TodoItemsService
			.EditAsync(TestContext, testItemId, dto)
			.Returns(TodoItemsServiceStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItem_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testItemId = 12412;
		TodoItemCreateDto dto = new("New name", "New description", null, 5, null);
		WebApplicationFactory.TodoItemsService
			.EditAsync(TestContext, testItemId, dto)
			.Returns(TodoItemsServiceStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PutTodoItem_InvalidCategoryIdStatus_Returns400()
	{
		// Arrange
		int testItemId = 12412;
		TodoItemCreateDto dto = new("New name", "New description", null, 5, null);
		WebApplicationFactory.TodoItemsService
			.EditAsync(TestContext, testItemId, dto)
			.Returns(TodoItemsServiceStatus.InvalidCategoryId);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", dto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItem_InvalidDto_Returns400()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemCreateDto invalidDto = new(string.Empty, string.Empty, DateTime.MinValue, 5, null);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItem_InvalidCategoryId_Returns400()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemCreateDto dto = new("New name", "New description", null, 5, 2);
		WebApplicationFactory.TodoItemsService
			.EditAsync(TestContext, testItemId, dto)
			.Returns(TodoItemsServiceStatus.InvalidCategoryId);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", dto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItem_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemCreateDto dto = new("Name", "Desc", DateTime.UtcNow.AddDays(5), 5, null);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task PutTodoItemState_ElementExists_Succeeds()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new(TodoItemState.Completed);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(TestContext, testItemId, dto)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}/state", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.UpdateStateAsync(TestContext, testItemId, dto);
	}

	[Test]
	public async Task PutTodoItemState_NotFoundStatus_Returns404()
	{
		// Arrange
		int testItemId = 12412;
		TodoItemUpdateStateDto dto = new(TodoItemState.Skipped);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(TestContext, testItemId, dto)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}/state", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PutTodoItemState_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testItemId = 12412;
		TodoItemUpdateStateDto dto = new(TodoItemState.Skipped);
		WebApplicationFactory.TodoItemsService
			.UpdateStateAsync(TestContext, testItemId, dto)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}/state", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PutTodoItemState_InvalidDto_Returns400()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new((TodoItemState)213);

		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}/state", dto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PutTodoItemState_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testItemId = 891349;
		TodoItemUpdateStateDto dto = new(TodoItemState.Active);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}/state", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task DeleteTodoItem_ElementExists_Succeeds()
	{
		// Arrange
		int testItemId = 504030;
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(TestContext, testItemId)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoItemsService
			.Received()
			.DeleteAsync(TestContext, testItemId);
	}

	[Test]
	public async Task DeleteTodoItem_NotFound_Returns404()
	{
		// Arrange
		int testItemId = 504030;
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(TestContext, testItemId)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteTodoItem_Forbidden_Returns403()
	{
		// Arrange
		int testItemId = 504030;
		WebApplicationFactory.TodoItemsService
			.DeleteAsync(TestContext, testItemId)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task DeleteTodoItem_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testItemId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/items/{testItemId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
