using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.IntegrationTests.Fixtures;
using NSubstitute;
using System.Net.Http.Json;
using System.Net;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Models.Auth;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoListMembersEndpointsTests : EndpointsFixture
{
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
		string testListId = "TestId";
		PaginationParameters parameters = new(Page: 2, PageSize: 20);
		TodoListMemberPreviewDto[] members =
		[
			new(124, new ApplicationUserPreviewDto("ID", "UserName"), new TodoListRolePreviewDto(700, "Admin")),
			new(512, new ApplicationUserPreviewDto("Rooster", "Inspector"), new TodoListRolePreviewDto(701, "Reviewer")),
		];
		WebApplicationFactory.TodoListMembersService
			.GetMembersAsync(testListId, parameters)
			.Returns(x => new(members, ((PaginationParameters)x[1]).Page,
			((PaginationParameters)x[1]).PageSize, 22));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/members?page={parameters.Page}&pageSize={parameters.PageSize}");

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
		string testListId = "TestId";
		using HttpClient client = CreateAuthorizedHttpClient();
		int page = -1;
		int pageSize = 0;

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/members?page={page}&pageSize={pageSize}");

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetTodoListMembersAsync_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/members?page=1&pageSize=20");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task GetTodoListMembersAsync_ListDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";

		WebApplicationFactory.TodoListMembersService
			.GetMembersAsync(testListId, Arg.Any<PaginationParameters>())
			.ReturnsNull();
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testListId}/members?page=1&pageSize=20");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task AddMember_ValidCall_Succeeds()
	{
		// Arrange
		string listId = "ListId";
		TodoListMemberAddDto dto = new("Name", 123);
		TodoListMemberMinimalViewDto outputDto = new(4124, dto.UserId, listId, dto.RoleId);
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(listId, dto)
			.Returns(new AddTodoListMemberResult(AddTodoListMemberResultStatus.Success, outputDto));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/members", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoListMembersService
			.Received()
			.AddMemberAsync(listId, dto);
	}

	[Test]
	public async Task AddMember_MemberAlreadyAdded_Fails()
	{
		// Arrange
		string listId = "ListId";
		TodoListMemberAddDto dto = new("Name", 123);
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(listId, dto)
			.Returns(new AddTodoListMemberResult(AddTodoListMemberResultStatus.UserAlreadyAdded));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/members", dto);

		// Assert that response code doesn't indicate success
		Assert.That(result.IsSuccessStatusCode, Is.False, "Unsuccessfull status code was expected.");
	}

	[Test]
	public async Task AddMember_TodoListDoesNotExist_Returns404()
	{
		// Arrange
		string listId = "ListId";
		TodoListMemberAddDto dto = new("Name", 123);
		WebApplicationFactory.TodoListMembersService
			.AddMemberAsync(listId, dto)
			.Returns(new AddTodoListMemberResult(AddTodoListMemberResultStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/members", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task AddMember_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string listId = "ListId";
		TodoListMemberAddDto dto = new("Name", 123);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/members", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task AddMember_InvalidDto_Returns400()
	{
		// Arrange
		string listId = "ListId";
		TodoListMemberAddDto invalidDto = new(string.Empty, -1);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync($"api/todo/{listId}/members", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task UpdateMemberRole_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 891349;
		TodoListMemberUpdateRoleDto dto = new(777);
		WebApplicationFactory.TodoListMembersService
			.UpdateMemberRoleAsync(testListId, testMemberId, dto)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/members/{testMemberId}", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that edit was called
		await WebApplicationFactory.TodoListMembersService
			.Received()
			.UpdateMemberRoleAsync(testListId, testMemberId, dto);
	}

	[Test]
	public async Task UpdateMemberRole_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 12412;
		TodoListMemberUpdateRoleDto dto = new(777);
		WebApplicationFactory.TodoListMembersService
			.UpdateMemberRoleAsync(testListId, testMemberId, dto)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/members/{testMemberId}", dto);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task UpdateMemberRole_InvalidDto_Returns400()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 891349;
		int invalidRoleId = 1241;
		TodoListMemberAddDto invalidDto = new("New name", invalidRoleId);
		using HttpClient client = CreateAuthorizedHttpClient();
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoListRole, int>(Arg.Is<int>(x => x == invalidRoleId))
			.Returns(false);

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/members/{testMemberId}", invalidDto);

		// Assert that response code is 400
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task UpdateMemberRole_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 891349;
		TodoListMemberAddDto dto = new("New name", 123);
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PutAsJsonAsync($"api/todo/{testListId}/members/{testMemberId}", dto);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task RemoveMember_ElementExists_Succeeds()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 504030;
		WebApplicationFactory.TodoListMembersService
			.RemoveMemberAsync(testListId, testMemberId)
			.Returns(true);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/members/{testMemberId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that delete was called
		await WebApplicationFactory.TodoListMembersService
			.Received()
			.RemoveMemberAsync(testListId, testMemberId);
	}

	[Test]
	public async Task RemoveMember_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 504030;
		WebApplicationFactory.TodoListMembersService
			.RemoveMemberAsync(testListId, testMemberId)
			.Returns(false);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/members/{testMemberId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}
	
	[Test]
	public async Task RemoveMember_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		string testListId = "TestId";
		int testMemberId = 504030;
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.DeleteAsync($"api/todo/{testListId}/members/{testMemberId}");

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
