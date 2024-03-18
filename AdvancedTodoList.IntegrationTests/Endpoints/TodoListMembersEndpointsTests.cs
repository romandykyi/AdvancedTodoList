using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoListMembersEndpointsTests : EndpointsFixture
{
	private readonly TodoListContext TestContext = new("TodoListId", TestUserId);

	[SetUp]
	public void SetUp()
	{
		// Make validation not to fail
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoListRole, int>(Arg.Any<int>())
			.Returns(true);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<ApplicationUser, string>(Arg.Any<string>())
			.Returns(true);
	}

	[Test]
	public async Task GetTodoListMembersAsync_ListExists_SucceedsAndReturnsItems()
	{
		// Arrange
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		TodoListMemberPreviewDto[] members =
		[
			new(124, new ApplicationUserPreviewDto("ID", "UserName"), new TodoListRolePreviewDto(700, "Admin")),
			new(512, new ApplicationUserPreviewDto("Rooster", "Inspector"), new TodoListRolePreviewDto(701, "Reviewer")),
		];
		WebApplicationFactory.TodoListMembersService
			.GetMembersAsync(TestContext, parameters, Arg.Any<TodoListMembersFilter>())
			.Returns(x => new ServiceResponse<Page<TodoListMemberPreviewDto>>(ServiceResponseStatus.Success,
			new(members, ((PaginationParameters)x[1]).Page, ((PaginationParameters)x[1]).PageSize, 22)));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/members?page={parameters.Page}&pageSize={parameters.PageSize}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid page was returned
		var returnedPage = await result.Content.ReadFromJsonAsync<Page<TodoListMemberPreviewDto>>();
		Assert.That(returnedPage, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(returnedPage.PageNumber, Is.EqualTo(parameters.Page));
			Assert.That(returnedPage.PageSize, Is.EqualTo(parameters.PageSize));
			Assert.That(returnedPage.Items, Is.EquivalentTo(members));
		});
	}

	[Test]
	public async Task GetTodoListMembersAsync_WrongPaginationParams_Returns400()
	{
		// Arrange
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/members?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetTodoListMembersAsync_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/members?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListMembersAsync_NotFoundStatus_Returns404()
	{
		// Arrange
		WebApplicationFactory.TodoListMembersService
			.GetMembersAsync(TestContext, Arg.Any<PaginationParameters>(), Arg.Any<TodoListMembersFilter>())
			.Returns(new ServiceResponse<Page<TodoListMemberPreviewDto>>(ServiceResponseStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/members?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetTodoListMembersAsync_ForbiddenStatus_Returns403()
	{
		// Arrange
		WebApplicationFactory.TodoListMembersService
			.GetMembersAsync(TestContext, Arg.Any<PaginationParameters>(), Arg.Any<TodoListMembersFilter>())
			.Returns(new ServiceResponse<Page<TodoListMemberPreviewDto>>(ServiceResponseStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/members?page=1&pageSize=20");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task AddMember_ValidCall_Succeeds()
	{
		// Arrange
		TodoListMemberAddDto dto = new("Name");
		TodoListMemberMinimalViewDto outputDto = new(4124, dto.UserId, TestContext.TodoListId, null);
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(TestContext, dto)
			.Returns(new AddTodoListMemberServiceResult(TodoListMemberServiceResultStatus.Success, outputDto));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/members", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoListMembersService
			.Received()
			.AddMemberAsync(TestContext, dto);
	}

	[Test]
	public async Task AddMember_MemberAlreadyAdded_Fails()
	{
		// Arrange
		TodoListMemberAddDto dto = new("Name");
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(TestContext, dto)
			.Returns(new AddTodoListMemberServiceResult(TodoListMemberServiceResultStatus.UserAlreadyAdded));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/members", dto);

		// Assert that response code doesn't indicate success
		Assert.That(result.IsSuccessStatusCode, Is.False, "Unsuccessfull status code was expected.");
	}

	[Test]
	public async Task AddMember_NotFoundStatus_Returns404()
	{
		// Arrange
		TodoListMemberAddDto dto = new("Name");
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(TestContext, dto)
			.Returns(new AddTodoListMemberServiceResult(TodoListMemberServiceResultStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/members", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task AddMember_ForbiddenStatus_Returns403()
	{
		// Arrange
		TodoListMemberAddDto dto = new("Name");
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(TestContext, dto)
			.Returns(new AddTodoListMemberServiceResult(TodoListMemberServiceResultStatus.Forbidden));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/members", dto);

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task AddMember_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		TodoListMemberAddDto dto = new("Name");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/members", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task AddMember_InvalidDto_Returns400()
	{
		// Arrange
		TodoListMemberAddDto invalidDto = new(string.Empty);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/members", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task UpdateMemberRole_ValidCall_Succeeds()
	{
		// Arrange
		int testMemberId = 891349;
		TodoListMemberUpdateRoleDto dto = new(777);
		WebApplicationFactory.TodoListMembersService
			.UpdateMemberRoleAsync(TestContext, testMemberId, dto)
			.Returns(TodoListMemberServiceResultStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoListMembersService
			.Received()
			.UpdateMemberRoleAsync(TestContext, testMemberId, dto);
	}

	[Test]
	public async Task UpdateMemberRole_ElementDoesNotExist_Returns404()
	{
		// Arrange
		int testMemberId = 12412;
		TodoListMemberUpdateRoleDto dto = new(777);
		WebApplicationFactory.TodoListMembersService
			.UpdateMemberRoleAsync(TestContext, testMemberId, dto)
			.Returns(TodoListMemberServiceResultStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task UpdateMemberRole_InvalidRoleStatus_Fails()
	{
		// Arrange
		int testMemberId = 12412;
		TodoListMemberUpdateRoleDto dto = new(777);
		WebApplicationFactory.TodoListMembersService
			.UpdateMemberRoleAsync(TestContext, testMemberId, dto)
			.Returns(TodoListMemberServiceResultStatus.InvalidRoleId);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}", dto);

		// Assert that response code indicates failure
		Assert.That(result.IsSuccessStatusCode, Is.False, "Unsuccessfull status code was expected.");
	}

	[Test]
	public async Task UpdateMemberRole_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testMemberId = 12412;
		TodoListMemberUpdateRoleDto dto = new(777);
		WebApplicationFactory.TodoListMembersService
			.UpdateMemberRoleAsync(TestContext, testMemberId, dto)
			.Returns(TodoListMemberServiceResultStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}", dto);

		// Assert
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task UpdateMemberRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testMemberId = 891349;
		TodoListMemberAddDto dto = new("New name");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task RemoveMember_ValidCall_Succeeds()
	{
		// Arrange
		int testMemberId = 504030;
		WebApplicationFactory.TodoListMembersService
			.RemoveMemberAsync(TestContext, testMemberId)
			.Returns(ServiceResponseStatus.Success);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoListMembersService
			.Received()
			.RemoveMemberAsync(TestContext, testMemberId);
	}

	[Test]
	public async Task RemoveMember_NotFoundStatus_Returns404()
	{
		// Arrange
		int testMemberId = 504030;
		WebApplicationFactory.TodoListMembersService
			.RemoveMemberAsync(TestContext, testMemberId)
			.Returns(ServiceResponseStatus.NotFound);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task RemoveMember_ForbiddenStatus_Returns403()
	{
		// Arrange
		int testMemberId = 504030;
		WebApplicationFactory.TodoListMembersService
			.RemoveMemberAsync(TestContext, testMemberId)
			.Returns(ServiceResponseStatus.Forbidden);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}");

		// Assert that response code is 403
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task RemoveMember_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		int testMemberId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/members/{testMemberId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
