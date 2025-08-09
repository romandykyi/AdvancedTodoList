using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using AdvancedTodoList.Application.Services.Definitions;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class JoinTodoListEndpointsTests : EndpointsFixture
{
	private const string TestCallerId = TestUserId;
	private const string TestInvitationLink = "abc";

	[Test]
	public async Task JoinByInvitationLink_ValidCall_Succeeds()
	{
		// Arrange
		TodoListMemberMinimalViewDto expectedDto = new(1, TestCallerId, "to-do-list-id", null);
		JoinByInvitationLinkResult response = new(JoinByInvitationLinkStatus.Success, expectedDto);
		WebApplicationFactory.InvitationLinksService
			.JoinAsync(TestCallerId, TestInvitationLink)
			.Returns(response);
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/join/{TestInvitationLink}", null);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that expected dto was returned
		var returnedDto = await result.Content.ReadFromJsonAsync<TodoListMemberMinimalViewDto>();
		Assert.That(returnedDto, Is.EqualTo(expectedDto));
	}

	[Test]
	public async Task JoinByInvitationLink_ExpiredLinkStatus_Fails()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.JoinAsync(TestCallerId, TestInvitationLink)
			.Returns(new JoinByInvitationLinkResult(JoinByInvitationLinkStatus.Expired));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/join/{TestInvitationLink}", null);

		// Assert that response code indicates failure
		Assert.That(result.IsSuccessStatusCode, Is.False, "Status code that indicated failure was expected.");
	}

	[Test]
	public async Task JoinByInvitationLink_UserIsAlreadyMemberStatus_Fails()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.JoinAsync(TestCallerId, TestInvitationLink)
			.Returns(new JoinByInvitationLinkResult(JoinByInvitationLinkStatus.UserIsAlreadyMember));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/join/{TestInvitationLink}", null);

		// Assert that response code indicates failure
		Assert.That(result.IsSuccessStatusCode, Is.False, "Status code that indicated failure was expected.");
	}

	[Test]
	public async Task JoinByInvitationLink_NotFoundStatus_Returns404()
	{
		// Arrange
		WebApplicationFactory.InvitationLinksService
			.JoinAsync(TestCallerId, TestInvitationLink)
			.Returns(new JoinByInvitationLinkResult(JoinByInvitationLinkStatus.NotFound));
		using HttpClient client = CreateAuthorizedHttpClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/join/{TestInvitationLink}", null);

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task JoinByInvitationLink_NoAuthHeaderProvided_Returns401()
	{
		// Arrange
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsync($"api/todo/join/{TestInvitationLink}", null);

		// Assert that response code is 401
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
