using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace AdvancedTodoList.IntegrationTests.Endpoints;

[TestFixture]
public class TodoItemCategoriesEndpointsTests : EndpointsFixture
{
    private readonly TodoListContext TestContext = new("TodoListId", TestUserId);

    [Test]
    public async Task GetTodoItemCategoriesAsync_ValidCall_SucceedsAndReturnsItems()
    {
        // Arrange
        PaginationParameters parameters = new(Page: 2, PageSize: 20);
        TodoItemCategoryViewDto[] categories =
        [
            new(1, "1"),
            new(2, "2"),
        ];
        string name = "n";
        WebApplicationFactory.TodoItemCategoriesService
            .GetCategoriesOfListAsync(TestContext, parameters, name)
            .Returns(x => new ServiceResponse<Page<TodoItemCategoryViewDto>>(
                ServiceResponseStatus.Success, new(categories, ((PaginationParameters)x[1]).Page,
                ((PaginationParameters)x[1]).PageSize, 22)));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories?page={parameters.Page}&pageSize={parameters.PageSize}&name={name}");

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that valid page was returned
        var returnedPage = await result.Content.ReadFromJsonAsync<Page<TodoItemCategoryViewDto>>();
        Assert.That(returnedPage, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPage.PageNumber, Is.EqualTo(parameters.Page));
            Assert.That(returnedPage.PageSize, Is.EqualTo(parameters.PageSize));
            Assert.That(returnedPage.Items, Is.EquivalentTo(categories));
        }
    }

    [Test]
    public async Task GetTodoItemCategoriesAsync_WrongPaginationParams_Returns400()
    {
        // Arrange
        using HttpClient client = CreateAuthorizedHttpClient();
        int page = -1;
        int pageSize = 0;

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories?page={page}&pageSize={pageSize}");

        // Assert that response code is 400
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetTodoItemCategoriesAsync_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories?page=1&pageSize=20");

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetTodoItemCategoriesAsync_NotFoundStatus_Returns404()
    {
        // Arrange
        WebApplicationFactory.TodoItemCategoriesService
            .GetCategoriesOfListAsync(TestContext, Arg.Any<PaginationParameters>())
            .Returns(x => new ServiceResponse<Page<TodoItemCategoryViewDto>>(ServiceResponseStatus.NotFound));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories?page=1&pageSize=20");

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetTodoItemCategoriesAsync_ForbiddenStatus_Returns403()
    {
        // Arrange
        WebApplicationFactory.TodoItemCategoriesService
            .GetCategoriesOfListAsync(TestContext, Arg.Any<PaginationParameters>())
            .Returns(x => new ServiceResponse<Page<TodoItemCategoryViewDto>>(ServiceResponseStatus.Forbidden));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories?page=1&pageSize=20");

        // Assert that response code is 403
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetTodoItemCategoryById_ValidCall_ReturnsElement()
    {
        // Arrange
        int testCategoryId = 777;
        TodoItemCategoryViewDto testDto = new(testCategoryId, "Name");

        WebApplicationFactory.TodoItemCategoriesService
            .GetByIdAsync(TestContext, testCategoryId)
            .Returns(new ServiceResponse<TodoItemCategoryViewDto>(ServiceResponseStatus.Success, testDto));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that valid object was returned
        var returnedDto = await result.Content.ReadFromJsonAsync<TodoItemCategoryViewDto>();
        Assert.That(returnedDto, Is.EqualTo(testDto));
    }

    [Test]
    public async Task GetTodoItemCategoryById_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        int testCategoryId = 777;
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetTodoItemCategoryById_NotFoundStatus_Returns404()
    {
        // Arrange
        int testCategoryId = 777;
        WebApplicationFactory.TodoItemCategoriesService
            .GetByIdAsync(TestContext, testCategoryId)
            .Returns(new ServiceResponse<TodoItemCategoryViewDto>(ServiceResponseStatus.NotFound));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetTodoItemCategoryById_ForbiddenStatus_Returns404()
    {
        // Arrange
        int testCategoryId = 777;
        WebApplicationFactory.TodoItemCategoriesService
            .GetByIdAsync(TestContext, testCategoryId)
            .Returns(new ServiceResponse<TodoItemCategoryViewDto>(ServiceResponseStatus.Forbidden));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.GetAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response code is 403
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PostTodoItemCategory_ValidCall_Succeeds()
    {
        // Arrange
        TodoItemCategoryCreateDto dto = new("Name");
        WebApplicationFactory.TodoItemCategoriesService
            .CreateAsync(TestContext, dto)
            .Returns(new ServiceResponse<TodoItemCategoryViewDto>(ServiceResponseStatus.Success));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories", dto);

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that create method was called
        await WebApplicationFactory.TodoItemCategoriesService
            .Received()
            .CreateAsync(TestContext, dto);
    }

    [Test]
    public async Task PostTodoItemCategory_NotFoundStatus_Returns404()
    {
        // Arrange
        TodoItemCategoryCreateDto dto = new("Name");
        WebApplicationFactory.TodoItemCategoriesService
            .CreateAsync(TestContext, dto)
            .Returns(new ServiceResponse<TodoItemCategoryViewDto>(ServiceResponseStatus.NotFound));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories", dto);

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PostTodoItemCategory_ForbiddenStatus_Returns403()
    {
        // Arrange
        TodoItemCategoryCreateDto dto = new("Name");
        WebApplicationFactory.TodoItemCategoriesService
            .CreateAsync(TestContext, dto)
            .Returns(new ServiceResponse<TodoItemCategoryViewDto>(ServiceResponseStatus.Forbidden));
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories", dto);

        // Assert that response code is 403
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PostTodoItemCategory_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        TodoItemCategoryCreateDto dto = new("Joker");
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories", dto);

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task PostTodoItemCategory_InvalidDto_Returns400()
    {
        // Arrange
        TodoItemCategoryCreateDto invalidDto = new(string.Empty);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PostAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories", invalidDto);

        // Assert that response code is 400
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PutTodoItemCategory_ValidCall_Succeeds()
    {
        // Arrange
        int testCategoryId = 891349;
        TodoItemCategoryCreateDto dto = new("New name");
        WebApplicationFactory.TodoItemCategoriesService
            .EditAsync(TestContext, testCategoryId, dto)
            .Returns(ServiceResponseStatus.Success);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}", dto);

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that edit was called
        await WebApplicationFactory.TodoItemCategoriesService
            .Received()
            .EditAsync(TestContext, testCategoryId, dto);
    }

    [Test]
    public async Task PutTodoItemCategory_NotFoundStatus_Returns404()
    {
        // Arrange
        int testCategoryId = 12412;
        TodoItemCategoryCreateDto dto = new("New name");
        WebApplicationFactory.TodoItemCategoriesService
            .EditAsync(TestContext, testCategoryId, dto)
            .Returns(ServiceResponseStatus.NotFound);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}", dto);

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PutTodoItemCategory_ForbiddenStatus_Returns403()
    {
        // Arrange
        int testCategoryId = 12412;
        TodoItemCategoryCreateDto dto = new("New name");
        WebApplicationFactory.TodoItemCategoriesService
            .EditAsync(TestContext, testCategoryId, dto)
            .Returns(ServiceResponseStatus.Forbidden);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}", dto);

        // Assert that response code is 403
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PutTodoItemCategory_InvalidDto_Returns400()
    {
        // Arrange
        int testCategoryId = 891349;
        TodoItemCategoryCreateDto invalidDto = new(string.Empty);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}", invalidDto);

        // Assert that response code is 400
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PutTodoItemCategory_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        int testCategoryId = 891349;
        TodoItemCategoryCreateDto dto = new("New name");
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.PutAsJsonAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}", dto);

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task DeleteTodoItemCategory_ValidCall_Succeeds()
    {
        // Arrange
        int testCategoryId = 504030;
        WebApplicationFactory.TodoItemCategoriesService
            .DeleteAsync(TestContext, testCategoryId)
            .Returns(ServiceResponseStatus.Success);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response indicates success
        result.EnsureSuccessStatusCode();
        // Assert that delete was called
        await WebApplicationFactory.TodoItemCategoriesService
            .Received()
            .DeleteAsync(TestContext, testCategoryId);
    }

    [Test]
    public async Task DeleteTodoItemCategory_NotFoundStatus_Returns404()
    {
        // Arrange
        int testCategoryId = 504030;
        WebApplicationFactory.TodoItemCategoriesService
            .DeleteAsync(TestContext, testCategoryId)
            .Returns(ServiceResponseStatus.NotFound);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response code is 404
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteTodoItemCategory_ForbiddenStatus_Returns403()
    {
        // Arrange
        int testCategoryId = 504030;
        WebApplicationFactory.TodoItemCategoriesService
            .DeleteAsync(TestContext, testCategoryId)
            .Returns(ServiceResponseStatus.Forbidden);
        using HttpClient client = CreateAuthorizedHttpClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response code is 403
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task DeleteTodoItemCategory_NoAuthHeaderProvided_Returns401()
    {
        // Arrange
        int testCategoryId = 504030;
        using HttpClient client = WebApplicationFactory.CreateClient();

        // Act: send the request
        var result = await client.DeleteAsync($"api/todo/{TestContext.TodoListId}/categories/{testCategoryId}");

        // Assert that response code is 401
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
