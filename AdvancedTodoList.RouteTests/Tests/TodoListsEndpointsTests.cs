using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using NSubstitute.ReturnsExtensions;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.RouteTests.Tests;

[TestFixture]
public class TodoListsEndpointsTests : RouteTest
{
	[Test]
	public async Task GetTodoListById_ElementExists_ReturnsElement()
	{
		// Arrange
		string testId = "TestId";
		TodoListGetByIdDto testDto = new(testId, "Test todo list", []);
		WebApplicationFactory.TodoListsService
			.GetByIdAsync(testId)
			.Returns(testDto);
		TodoListCreateDto dto = new("Test");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testId}");

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that valid object was returned
		var returnedDto = await result.Content.ReadFromJsonAsync<TodoListGetByIdDto>();
		Assert.That(returnedDto, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(returnedDto.Id, Is.EqualTo(testDto.Id));
			Assert.That(returnedDto.Name, Is.EqualTo(testDto.Name));
		});
	}

	[Test]
	public async Task GetTodoListById_ElementDoesNotExist_Returns404()
	{
		// Arrange
		string testId = "TestId";
		WebApplicationFactory.TodoListsService
			.GetByIdAsync(testId)
			.ReturnsNull();
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.GetAsync($"api/todo/{testId}");

		// Assert that response code is 404
		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostTodoList_ValidCall_Succeeds()
	{
		// Arrange
		WebApplicationFactory.TodoListsService
			.CreateAsync(Arg.Any<TodoListCreateDto>())
			.Returns(new TodoList() { Id = "TestID" });
		TodoListCreateDto dto = new("Test");
		using HttpClient client = WebApplicationFactory.CreateClient();

		// Act: send the request
		var result = await client.PostAsJsonAsync("api/todo", dto);

		// Assert that response indicates success
		result.EnsureSuccessStatusCode();
		// Assert that create method was called
		await WebApplicationFactory.TodoListsService
			.Received()
			.CreateAsync(dto);
	}
}
