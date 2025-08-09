using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications.Filters;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoListsEndpointsTests : EndpointsFixture
{
    private readonly TodoListContext TestContext = new("TestTodoListId", TestUserId);

    [Test]
    public async Task GetTodoListsOfCaller_ValidCall_SucceedsAndReturnsItems()
    {
        // Arrange
        PaginationParameters parameters = new(Page: 2, PageSize: 20);
        TodoListPreviewDto[] items =
        [
            new("123", "Abc"),
            new("456", "Def"),
        ];
        TodoListsFilter filter = new(Name: "abc");
        WebApplicationFactory.TodoListsService
            .GetListsOfUserAsync(TestUserId, parameters, filter)
            .Returns(x => new(items, ((PaginationParameters)x[1]).Page,
                ((PaginationParameters)x[1]).PageSize, 22));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo?page={parameters.Page}&pageSize={parameters.PageSize}&name={filter.Name}");

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that valid page was returned
        var returnedPage = await result.Content.ReadFromJsonAsync<Page<TodoListPreviewDto>>();
        Assert.That(returnedPage, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPage.PageNumber, Is.EqualTo(parameters.Page));
            Assert.That(returnedPage.PageSize, Is.EqualTo(parameters.PageSize));
            Assert.That(returnedPage.Items, Is.EquivalentTo(items));
        }
    }

    [Test]
    public async Task GetTodoListsOfCaller_WrongPaginationParams_Returns400()
    {
        // Arrange
        using HttpClient client = CreateAuthorizedHttpClient();
        int page = -1;
        int pageSize = 0;

        // Act: send the request
        var result = await client.GetAsync($"api/todo?page={page}&pageSize={pageSize}");

        // Assert that response code is 400
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetTodoListsOfCaller_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo?page=1&pageSize=20");

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetTodoListById_ElementExists_ReturnsElement()
    {
        // Arrange
        TodoListGetByIdDto testDto = new(TestContext.TodoListId, "Test todo list", "", new("Id", "User"));
        WebApplicationFactory.TodoListsService
            .GetByIdAsync(TestContext)
            .Returns(new ServiceResponse<TodoListGetByIdDto>(ServiceResponseStatus.Success, testDto));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}");

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
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}");

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetTodoListById_ElementDoesNotExist_Returns404()
    {
        // Arrange
        WebApplicationFactory.TodoListsService
            .GetByIdAsync(TestContext)
            .Returns(new ServiceResponse<TodoListGetByIdDto>(ServiceResponseStatus.NotFound));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}");

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetTodoListById_UserHasNoPermission_Returns401()
    {
        // Arrange
        WebApplicationFactory.TodoListsService
            .GetByIdAsync(TestContext)
            .Returns(new ServiceResponse<TodoListGetByIdDto>(ServiceResponseStatus.Forbidden));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}");

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
        TodoListCreateDto dto = new("New name", "New description");

        WebApplicationFactory.TodoListsService
            .EditAsync(TestContext, dto)
            .Returns(ServiceResponseStatus.Success);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}", dto);

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that edit was called
        await WebApplicationFactory.TodoListsService
            .Received()
            .EditAsync(TestContext, dto);
    }

    [Test]
    public async Task PutTodoList_InvalidDto_Returns400()
    {
        // Arrange
        TodoListCreateDto invalidDto = new(string.Empty, string.Empty);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}", invalidDto);

        // Assert that response code is 400
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PutTodoList_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        TodoListCreateDto invalidDto = new("Name", "Descr");
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}", invalidDto);

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task PutTodoList_ElementDoesNotExist_Returns404()
    {
        // Arrange
        TodoListCreateDto dto = new("New name", "New description");

        WebApplicationFactory.TodoListsService
            .EditAsync(TestContext, dto)
            .Returns(ServiceResponseStatus.NotFound);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}", dto);

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PutTodoList_UserHasNoPermission_Returns403()
    {
        // Arrange
        TodoListCreateDto dto = new("New name", "New description");

        WebApplicationFactory.TodoListsService
            .EditAsync(TestContext, dto)
            .Returns(ServiceResponseStatus.Forbidden);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}", dto);

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task DeleteTodoList_ElementExists_Succeeds()
    {
        // Arrange
        WebApplicationFactory.TodoListsService
            .DeleteAsync(TestContext)
            .Returns(ServiceResponseStatus.Success);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}");

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that delete was called
        await WebApplicationFactory.TodoListsService
            .Received()
            .DeleteAsync(TestContext);
    }

    [Test]
    public async Task DeleteTodoList_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}");

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task DeleteTodoList_ElementDoesNotExist_Returns404()
    {
        // Arrange
        WebApplicationFactory.TodoListsService
            .DeleteAsync(TestContext)
            .Returns(ServiceResponseStatus.NotFound);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}");

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteTodoList_UserHasNoPermission_Returns401()
    {
        // Arrange
        WebApplicationFactory.TodoListsService
            .DeleteAsync(TestContext)
            .Returns(ServiceResponseStatus.Forbidden);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}");

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}
