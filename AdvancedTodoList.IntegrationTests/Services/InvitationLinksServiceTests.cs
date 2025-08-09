using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class InvitationLinksServiceTests : BusinessLogicFixture
{
    private const string TestCallerId = "CallerId";
    private const string TestTodoListId = "ListId";
    private readonly TodoListContext TestContext = new("ListId", "TestUserId");
    private IInvitationLinksService _service;

    [SetUp]
    public void SetUp()
    {
        _service = ServiceScope.ServiceProvider.GetService<IInvitationLinksService>()!;
    }

    [Test]
    public async Task JoinAsync_ValidCall_ReturnsSuccess()
    {
        // Arrange
        InvitationLink link = TestModels.CreateTestInvitationLink(TestTodoListId);
        WebApplicationFactory.InvitationLinksRepository
            .FindAsync(link.Value)
            .Returns(link);
        WebApplicationFactory.TodoListMembersRepository
            .FindAsync(TestTodoListId, TestCallerId)
            .ReturnsNull();

        // Act
        var response = await _service.JoinAsync(TestCallerId, link.Value);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Status, Is.EqualTo(JoinByInvitationLinkStatus.Success));
            Assert.That(response.Dto, Is.Not.Null);
        });
    }

    [Test]
    public async Task JoinAsync_LinkDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        string linkValue = "0wnh0w93n";
        WebApplicationFactory.InvitationLinksRepository
            .FindAsync(linkValue)
            .ReturnsNull();

        // Act
        var response = await _service.JoinAsync(TestCallerId, linkValue);

        // Assert
        Assert.That(response.Status, Is.EqualTo(JoinByInvitationLinkStatus.NotFound));
    }

    [Test]
    public async Task JoinAsync_ExpiredLink_ReturnsExpired()
    {
        // Arrange
        InvitationLink link = TestModels.CreateTestInvitationLink(
            TestTodoListId, DateTime.UtcNow.AddDays(-30));
        WebApplicationFactory.InvitationLinksRepository
            .FindAsync(link.Value)
            .Returns(link);

        // Act
        var response = await _service.JoinAsync(TestCallerId, link.Value);

        // Assert
        Assert.That(response.Status, Is.EqualTo(JoinByInvitationLinkStatus.Expired));
    }

    [Test]
    public async Task JoinAsync_CallerIsTodoListMember_ReturnsUserIsAlreadyMember()
    {
        // Arrange
        InvitationLink link = TestModels.CreateTestInvitationLink(TestTodoListId);
        WebApplicationFactory.InvitationLinksRepository
            .FindAsync(link.Value)
            .Returns(link);
        WebApplicationFactory.TodoListMembersRepository
            .FindAsync(TestTodoListId, TestCallerId)
            .Returns(new TodoListMember() { TodoListId = TestTodoListId, UserId = TestCallerId });

        // Act
        var response = await _service.JoinAsync(TestCallerId, link.Value);

        // Assert
        Assert.That(response.Status, Is.EqualTo(JoinByInvitationLinkStatus.UserIsAlreadyMember));
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task GetPageAsync_ListExists_AppliesSpecification(bool addMembersPermission, bool manageLinksPermission)
    {
        // Arrange
        PaginationParameters parameters = new(2, 5);
        RolePermissions validPermissions = new(
            AddMembers: addMembersPermission,
            ManageInvitationLinks: manageLinksPermission);
        Page<InvitationLinkDto> page = new([], parameters.Page, parameters.PageSize, 0);
        WebApplicationFactory.EntityExistenceChecker
            .ExistsAsync<TodoList, string>(TestContext.TodoListId)
            .Returns(true);
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(x => ((Func<RolePermissions, bool>)x[1])(validPermissions));
        WebApplicationFactory.InvitationLinksRepository
            .GetPageAsync<InvitationLinkDto>(Arg.Any<PaginationParameters>(),
            Arg.Any<ISpecification<InvitationLink>>())
            .Returns(page);

        // Act
        var result = await _service.GetInvitationLinksAsync(TestContext, parameters);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Success));
        await WebApplicationFactory.InvitationLinksRepository
            .Received()
            .GetPageAsync<InvitationLinkDto>(parameters, Arg.Any<InvitationLinksSpecification>());
    }

    [Test]
    public async Task GetPageAsync_ListDoesNotExist_ReturnsNotFoundStatus()
    {
        // Arrange
        PaginationParameters parameters = new(2, 5);
        WebApplicationFactory.EntityExistenceChecker
            .ExistsAsync<TodoList, string>(TestContext.TodoListId)
            .Returns(false);
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(x => true);

        // Act
        var result = await _service.GetInvitationLinksAsync(TestContext, parameters);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
    }

    [Test]
    public async Task GetPageAsync_UserIsNotMember_ReturnsForbidden()
    {
        // Arrange
        PaginationParameters parameters = new(2, 5);
        WebApplicationFactory.EntityExistenceChecker
            .ExistsAsync<TodoList, string>(TestContext.TodoListId)
            .Returns(true);
        WebApplicationFactory.PermissionsChecker
            .IsMemberOfListAsync(TestContext)
            .Returns(false);

        // Act
        var result = await _service.GetInvitationLinksAsync(TestContext, parameters);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
    }


    [Test]
    public async Task CreateAsync_ValidCall_AddsEntityToDb()
    {
        // Arrange
        RolePermissions validPermissions = new(AddMembers: true);
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(x => ((Func<RolePermissions, bool>)x[1])(validPermissions));
        WebApplicationFactory.EntityExistenceChecker
            .ExistsAsync<TodoList, string>(TestContext.TodoListId)
            .Returns(true);
        WebApplicationFactory.InvitationLinksRepository
            .AddAsync(Arg.Any<InvitationLink>())
            .Returns(Task.FromResult);

        // Act
        var response = await _service.CreateAsync(TestContext);

        // Assert
        Assert.That(response.Status, Is.EqualTo(ServiceResponseStatus.Success));
        await WebApplicationFactory.InvitationLinksRepository
            .Received()
            .AddAsync(Arg.Is<InvitationLink>(x => x.TodoListId == TestContext.TodoListId));
        Assert.That(response.Result, Is.Not.Null);
        // Refresh token is not a default base64 string
        byte[] base64Link = Convert.FromBase64String(response.Result.Value);
        Assert.That(base64Link.All(x => x == 0), Is.False);
    }

    [Test]
    public async Task CreateAsync_TodoListDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(true);
        WebApplicationFactory.EntityExistenceChecker
            .ExistsAsync<TodoList, string>(TestContext.TodoListId)
            .Returns(false);

        // Act: call the method
        var result = await _service.CreateAsync(TestContext);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
    }

    [Test]
    public async Task CreateAsync_UserHasNoPermssion_ReturnsForbidden()
    {
        // Arrange
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(false);
        WebApplicationFactory.EntityExistenceChecker
            .ExistsAsync<TodoList, string>(TestContext.TodoListId)
            .Returns(true);

        // Act: call the method
        var result = await _service.CreateAsync(TestContext);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
    }

    [Test]
    public async Task DeleteAsync_ValidCall_Succeeds()
    {
        // Arrange
        var entity = TestModels.CreateTestInvitationLink(TestContext.TodoListId);
        RolePermissions validPermissions = new(ManageInvitationLinks: true);
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(x => ((Func<RolePermissions, bool>)x[1])(validPermissions));
        WebApplicationFactory.InvitationLinksRepository
            .GetByIdAsync(entity.Id)
            .Returns(entity);
        WebApplicationFactory.InvitationLinksRepository
            .DeleteAsync(Arg.Any<InvitationLink>())
            .Returns(Task.FromResult);

        // Act
        var result = await _service.DeleteAsync(TestContext, entity.Id);

        // Assert
        Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
        // Assert that delete was called
        await WebApplicationFactory.InvitationLinksRepository
            .Received()
            .DeleteAsync(Arg.Is<InvitationLink>(x => x.Id == entity.Id));
    }

    [Test]
    public async Task DeleteAsync_EntityDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        int entityId = 500;
        WebApplicationFactory.InvitationLinksRepository
            .GetByIdAsync(entityId)
            .ReturnsNull();

        // Act
        var result = await _service.DeleteAsync(TestContext, entityId);

        // Assert
        Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
    }

    [Test]
    public async Task DeleteAsync_InvalidTodoListId_ReturnsNotFound()
    {
        // Arrange
        int entityId = 500;
        var entity = TestModels.CreateTestInvitationLink("Wrong to-do list ID");
        WebApplicationFactory.InvitationLinksRepository
            .GetByIdAsync(entity.Id)
            .Returns(entity);
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(x => false);

        // Act
        var result = await _service.DeleteAsync(TestContext, entityId);

        // Assert
        Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
    }

    [Test]
    public async Task DeleteAsync_UserHasNoPermission_ReturnsForbidden()
    {
        // Arrange
        var entity = TestModels.CreateTestInvitationLink(TestContext.TodoListId);
        WebApplicationFactory.InvitationLinksRepository
            .GetByIdAsync(entity.Id)
            .Returns(entity);
        WebApplicationFactory.PermissionsChecker
            .HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
            .Returns(x => false);

        // Act
        var result = await _service.DeleteAsync(TestContext, entity.Id);

        // Assert
        Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
    }
}
