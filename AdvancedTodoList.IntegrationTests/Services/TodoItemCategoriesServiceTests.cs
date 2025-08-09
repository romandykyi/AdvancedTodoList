using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Core.Specifications.Todo;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoItemCategoriesServiceTests : BusinessLogicFixture
{
    private ITodoItemCategoriesService _service;

    private readonly TodoListContext TestContext = new("TodoListId", "TestUserId");

    [SetUp]
    public void SetUp()
    {
        _service = ServiceScope.ServiceProvider.GetService<ITodoItemCategoriesService>()!;
    }

    [Test]
    public async Task GetCategoriesOfListAsync_ListExists_AppliesTodoItemsSpecification()
    {
        // Arrange
        PaginationParameters parameters = new(2, 5);
        Page<TodoItemCategoryViewDto> page = new([], 1, 1, 1);
        const string name = "n";
        WebApplicationFactory.TodoItemCategoriesHelperService
            .GetPageAsync<TodoItemCategoryViewDto>(TestContext, Arg.Any<ISpecification<TodoItemCategory>>(), Arg.Any<PaginationParameters>())
            .Returns(new ServiceResponse<Page<TodoItemCategoryViewDto>>(ServiceResponseStatus.Success, page));

        // Act
        var result = await _service.GetCategoriesOfListAsync(TestContext, parameters, name);

        // Assert
        Assert.That(result, Is.Not.Null);
        await WebApplicationFactory.TodoItemCategoriesHelperService
            .Received()
            .GetPageAsync<TodoItemCategoryViewDto>(TestContext,
            Arg.Is<TodoListDependantEntitiesSpecification<TodoItemCategory>>(
                x => x.TodoListId == TestContext.TodoListId && x.Name == name),
            Arg.Any<PaginationParameters>());
    }

    [Test]
    public async Task IsCategoryValidForContext_CategoryIdIsNull_ReturnsTrue()
    {
        // Act
        var result = await _service.IsCategoryValidForContext(TestContext, null);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsCategoryValidForContext_CategoryIsValid_ReturnsTrue()
    {
        // Arrange
        int categoryId = 1;
        var category = TestModels.CreateTestTodoItemCategory(TestContext.TodoListId);
        WebApplicationFactory.TodoItemCategoriesRepository
            .GetByIdAsync(categoryId)
            .Returns(category);

        // Act
        var result = await _service.IsCategoryValidForContext(TestContext, categoryId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsCategoryValidForContext_CategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        int categoryId = 1;
        WebApplicationFactory.TodoItemCategoriesRepository
            .GetByIdAsync(categoryId)
            .ReturnsNull();

        // Act
        var result = await _service.IsCategoryValidForContext(TestContext, categoryId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsCategoryValidForContext_WrongTodoListId_ReturnsFalse()
    {
        // Arrange
        int categoryId = 1;
        var category = TestModels.CreateTestTodoItemCategory("Wrong to-do list ID");
        WebApplicationFactory.TodoItemCategoriesRepository
            .GetByIdAsync(categoryId)
            .Returns(category);

        // Act
        var result = await _service.IsCategoryValidForContext(TestContext, categoryId);

        // Assert
        Assert.That(result, Is.False);
    }
}
