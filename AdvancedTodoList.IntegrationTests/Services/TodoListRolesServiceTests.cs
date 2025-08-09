using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Core.Specifications.Todo;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using AdvancedTodoList.Application.Services.Definitions;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListRolesServiceTests : BusinessLogicFixture
{
	private ITodoListRolesService _service;
	private readonly TodoListContext TestContext = new("TodoListId", "CallerId");
	private readonly TodoListRoleCreateDto TestInputDto = new("A role", 3, RolePermissions.All);

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoListRolesService>()!;
	}

	[Test]
	public async Task GetRolesOfListAsync_ListExists_AppliesTodoItemsSpecification()
	{
		// Arrange
		PaginationParameters parameters = new(2, 5);
		Page<TodoListRolePreviewDto> page = new([], 1, 1, 1);
		const string name = "Role #1";
		WebApplicationFactory.TodoRolesHelperService
			.GetPageAsync<TodoListRolePreviewDto>(TestContext, Arg.Any<ISpecification<TodoListRole>>(), Arg.Any<PaginationParameters>())
			.Returns(new ServiceResponse<Page<TodoListRolePreviewDto>>(ServiceResponseStatus.Success, page));

		// Act
		var result = await _service.GetRolesOfListAsync(TestContext, parameters, name);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoRolesHelperService
			.Received()
			.GetPageAsync<TodoListRolePreviewDto>(TestContext,
			Arg.Is<TodoListDependantEntitiesSpecification<TodoListRole>>(
				x => x.TodoListId == TestContext.TodoListId && x.Name == name),
			Arg.Any<PaginationParameters>());
	}

	[Test]
	public async Task CreateAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, TestInputDto.Priority, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);

		// Act
		var response = await _service.CreateAsync(TestContext, TestInputDto);

		// Arrange
		Assert.That(response.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task CreateAsync_UserHasPermission_CallsHelperService()
	{
		// Arrange
		RolePermissions validPermissions = new(EditRoles: true);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, TestInputDto.Priority, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[2])(validPermissions));
		WebApplicationFactory.TodoRolesHelperService
			.CreateAsync<TodoListRoleCreateDto, TodoListRoleViewDto>(
			Arg.Any<TodoListContext>(), Arg.Any<TodoListRoleCreateDto>(), null)
			.Returns(new ServiceResponse<TodoListRoleViewDto>(ServiceResponseStatus.Success));

		// Act
		var response = await _service.CreateAsync(TestContext, TestInputDto);

		// Arrange
		await WebApplicationFactory.TodoRolesHelperService
			.Received(1)
			.CreateAsync<TodoListRoleCreateDto, TodoListRoleViewDto>(TestContext, TestInputDto, null);
	}

	[Test]
	public async Task EditAsync_EntityExists_Succeeds()
	{
		// Arrange
		var role = TestModels.CreateTestRole(TestContext.TodoListId);
		RolePermissions validPermissions = new(EditRoles: true);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, Math.Min(TestInputDto.Priority, role.Priority), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[2])(validPermissions));
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(role.Id)
			.Returns(role);
		WebApplicationFactory.TodoListRolesRepository
			.UpdateAsync(Arg.Any<TodoListRole>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.EditAsync(TestContext, role.Id, TestInputDto);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that update was called
		await WebApplicationFactory.TodoListRolesRepository
			.Received()
			.UpdateAsync(Arg.Is<TodoListRole>(x =>
			x.Id == role.Id && x.Permissions == TestInputDto.Permissions && x.TodoListId == TestContext.TodoListId));
	}

	[Test]
	public async Task UpdateAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		int roleId = 500;
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(roleId)
			.ReturnsNull();

		// Act
		var result = await _service.EditAsync(TestContext, roleId, TestInputDto);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task UpdateAsync_WrongTodoListId_ReturnsNotFound()
	{
		// Arrange
		var role = TestModels.CreateTestRole("Wrong to-do list ID");
		int roleId = 500;
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TodoListRole, int>(TestContext,
			Arg.Any<TodoListRole>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(role.Id)
			.Returns(role);

		// Act
		var result = await _service.EditAsync(TestContext, roleId, TestInputDto);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task UpdateAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		var role = TestModels.CreateTestRole(TestContext.TodoListId);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, Arg.Any<int>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(role.Id)
			.Returns(role);

		// Act
		var result = await _service.EditAsync(TestContext, role.Id, TestInputDto);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task DeleteAsync_ValidCall_Succeeds()
	{
		// Arrange
		var role = TestModels.CreateTestRole(TestContext.TodoListId);
		RolePermissions validPermissions = new(EditRoles: true);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, role.Priority, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[2])(validPermissions));
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(role.Id)
			.Returns(role);
		WebApplicationFactory.TodoListRolesRepository
			.DeleteAsync(Arg.Any<TodoListRole>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.DeleteAsync(TestContext, role.Id);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that delete was called
		await WebApplicationFactory.TodoListRolesRepository
			.Received()
			.DeleteAsync(Arg.Is<TodoListRole>(x => x.Id == role.Id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		int roleId = 500;
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(roleId)
			.ReturnsNull();

		// Act
		var result = await _service.DeleteAsync(TestContext, roleId);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task DeleteAsync_InvalidTodoListId_ReturnsNotFound()
	{
		// Arrange
		int roleId = 500;
		var role = TestModels.CreateTestRole("Wrong to-do list ID");
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(role.Id)
			.Returns(role);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, Math.Min(TestInputDto.Priority, role.Priority), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);

		// Act
		var result = await _service.DeleteAsync(TestContext, roleId);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task DeleteAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		var role = TestModels.CreateTestRole(TestContext.TodoListId);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(role.Id)
			.Returns(role);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionOverRoleAsync(TestContext, Math.Min(TestInputDto.Priority, role.Priority), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);

		// Act
		var result = await _service.DeleteAsync(TestContext, role.Id);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}
}
