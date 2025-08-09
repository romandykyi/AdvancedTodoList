using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using AdvancedTodoList.Application.Services.Definitions;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class InvitationLinksEndpointsTests : EndpointsFixture
{
	private readonly TodoListContext TestContext = new("TodoListId", TestUserId);

	[Test]
	public async Task GetInvitationLinks_ValidCall_SucceedsAndReturnsLinks()
	{
		// Arrange
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		InvitationLinkDto[] invitationLinks =
		[
			new(1, "1", DateTime.UtcNow.AddDays(-30)),
			new(2, "2", DateTime.UtcNow),
		];
		WebApplicationFactory.InvitationLinksService
			.GetInvitationLinksAsync(TestContext, parameters)
			.Returns(x => new ServiceResponse<Page<InvitationLinkDto>>(
				ServiceResponseStatus.Success, new(invitationLinks, ((PaginationParameters)x[1]).Page,
				((PaginationParameters)x[1]).PageSize, 22)));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/invitationLinks?page={parameters.Page}&pageSize={parameters.PageSize}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid page was returned
		var returnedPage = await result.Content.ReadFromJsonAsync<Page<InvitationLinkDto>>();
		Assert.That(returnedPage, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(returnedPage.PageNumber, Is.EqualTo(parameters.Page));
			Assert.That(returnedPage.PageSize, Is.EqualTo(parameters.PageSize));
			Assert.That(returnedPage.Items, Is.EquivalentTo(invitationLinks));
		});
	}

	[Test]
	public async Task GetInvitationLinks_WrongPaginationParams_Returns400()
	{
		// Arrange
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/invitationLinks?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetInvitationLinks_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/invitationLinks?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetInvitationLinks_NotFoundStatus_Returns404()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.GetInvitationLinksAsync(TestContext, Arg.Any<PaginationParameters>())
			.Returns(x => new ServiceResponse<Page<InvitationLinkDto>>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/invitationLinks?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetInvitationLinks_ForbiddenStatus_Returns403()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.GetInvitationLinksAsync(TestContext, Arg.Any<PaginationParameters>())
			.Returns(x => new ServiceResponse<Page<InvitationLinkDto>>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/invitationLinks?page=1&pageSize=20");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostInvitationLink_ValidCall_Succeeds()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.CreateAsync(TestContext)
			.Returns(new ServiceResponse<InvitationLinkDto>(ServiceResponseStatus.Success));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/{TestContext.TodoListId}/invitationLinks", null);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.InvitationLinksService
			.Received()
			.CreateAsync(TestContext);
	}

	[Test]
	public async Task PostInvitationLink_NotFoundStatus_Returns404()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.CreateAsync(TestContext)
			.Returns(new ServiceResponse<InvitationLinkDto>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/{TestContext.TodoListId}/invitationLinks", null);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostInvitationLink_ForbiddenStatus_Returns403()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.CreateAsync(TestContext)
			.Returns(new ServiceResponse<InvitationLinkDto>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/{TestContext.TodoListId}/invitationLinks", null);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task PostInvitationLink_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/{TestContext.TodoListId}/invitationLinks", null);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task DeleteInvitationLink_ValidCall_Succeeds()
	{
		// Arrange
		int testInvitationLinkId = 504030;
		WebApplicationFactory.InvitationLinksService
			.DeleteAsync(TestContext, testInvitationLinkId)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/invitationLinks/{testInvitationLinkId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.InvitationLinksService
			.Received()
			.DeleteAsync(TestContext, testInvitationLinkId);
	}

	[Test]
	public async Task DeleteInvitationLink_NotFoundStatus_Returns404()
	{
		// Arrange
		int testInvitationLinkId = 504030;
		WebApplicationFactory.InvitationLinksService
			.DeleteAsync(TestContext, testInvitationLinkId)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/invitationLinks/{testInvitationLinkId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task DeleteInvitationLink_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testInvitationLinkId = 504030;
		WebApplicationFactory.InvitationLinksService
			.DeleteAsync(TestContext, testInvitationLinkId)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/invitationLinks/{testInvitationLinkId}");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task DeleteInvitationLink_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testInvitationLinkId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/invitationLinks/{testInvitationLinkId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
