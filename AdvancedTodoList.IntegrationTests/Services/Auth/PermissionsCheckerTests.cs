using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;

namespace AdvancedTodoList.IntegrationTests.Services.Auth;

public class PermissionsCheckerTests : DataAccessFixture
{
	private IPermissionsChecker _permissionsChecker;

	[SetUp]
	public void SetUp()
	{
		_permissionsChecker = ServiceScope.ServiceProvider.GetService<IPermissionsChecker>()!;
	}

	private async Task<ApplicationUser> CreateTestUserAsync()
	{
		ApplicationUser user = TestModels.CreateTestUser();
		DbContext.Add(user);
		await DbContext.SaveChangesAsync();
		return user;
	}
	private async Task<TodoList> CreateTestTodoListAsync()
	{
		TodoList todoList = TestModels.CreateTestTodoList();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		return todoList;
	}
	private async Task<TodoListRole> CreateTestRoleAsync(string todoListId, int priority, RolePermissions permissions)
	{
		TodoListRole role = new()
		{
			Name = "Role",
			TodoListId = todoListId,
			Priority = priority,
			Permissions = permissions
		};
		DbContext.Add(role);
		await DbContext.SaveChangesAsync();

		return role;
	}
	private async Task<TodoListMember> CreateTestMemberAsync()
	{
		var user = await CreateTestUserAsync();
		var list = await CreateTestTodoListAsync();

		return await CreateTestMemberAsync(user.Id, list.Id, null);
	}
	private async Task<TodoListMember> CreateTestMemberAsync(string userId, string todoListId, int? roleId)
	{
		TodoListMember member = new()
		{
			UserId = userId,
			TodoListId = todoListId,
			RoleId = roleId
		};
		DbContext.Add(member);
		await DbContext.SaveChangesAsync();
		return member;
	}

	private TodoList GetTestEntity(string? ownerId)
	{
		return new TodoList()
		{
			Name = "N",
			Description = "D",
			OwnerId = ownerId
		};
	}

	[Test]
	public async Task IsMemberOfListAsync_UserIsMember_ReturnsTrue()
	{
		// Arrange
		var member = await CreateTestMemberAsync();
		TodoListContext context = new(member.TodoListId, member.UserId);

		// Act
		bool result = await _permissionsChecker.IsMemberOfListAsync(context);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task IsMemberOfListAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var user = await CreateTestUserAsync();
		var list = await CreateTestTodoListAsync();
		TodoListContext context = new(list.Id, user.Id);

		// Act
		bool result = await _permissionsChecker.IsMemberOfListAsync(context);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionAsync_UserHasPermission_ReturnsTrue()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(AddMembers: true));
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(context, x => x.AddMembers);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task HasPermissionAsync_UserHasNoPermission_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(AddMembers: true, AddItems: false));
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(context, x => x.AddItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionAsync_UserHasNoRole_ReturnsFalse()
	{
		// Arrange
		var member = await CreateTestMemberAsync();
		TodoListContext context = new(member.TodoListId, member.UserId);

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(context, x => x.AddItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var user = await CreateTestUserAsync();
		var list = await CreateTestTodoListAsync();
		TodoListContext context = new(list.Id, user.Id);

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(context, x => x.AddItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserHasPermission_ReturnsTrue()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(EditItems: true));
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		var testEntity = GetTestEntity("some-random-user-id");
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(context, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserIsMemberAndOwnsEntity_ReturnsTrue()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(EditItems: false));
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		var testEntity = GetTestEntity(user.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(context, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserHasNoPermission_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(EditItems: false));
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		var testEntity = GetTestEntity("some-random-user-id");
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(context, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserHasNoRole_ReturnsFalse()
	{
		// Arrange
		var member = await CreateTestMemberAsync();
		var testEntity = GetTestEntity("some-random-user-id");
		TodoListContext context = new(member.TodoListId, member.UserId);

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(context, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var testEntity = GetTestEntity("some-random-user-id");
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(context, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserIsNotMemberButOwnsEntity_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var testEntity = GetTestEntity(user.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(context, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserHasPermission_ReturnsTrue()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(EditRoles: true));
		const int otherRolePriority = 4;
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(context, otherRolePriority, x => x.EditRoles);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserHasNoPermission_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(EditRoles: false));
		const int otherRolePriority = 4;
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(context, otherRolePriority, x => x.EditRoles);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserHasNoRole_ReturnsFalse()
	{
		// Arrange
		var member = await CreateTestMemberAsync();
		const int otherRolePriority = 4;
		TodoListContext context = new(member.TodoListId, member.UserId);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(context, otherRolePriority, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		const int otherRolePriority = 4;
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(context, otherRolePriority, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserHasPermissionButHasLowerRole_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var role = await CreateTestRoleAsync(todoList.Id, 3, new(EditRoles: true));
		const int otherRolePriority = 2;
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);
		TodoListContext context = new(todoList.Id, user.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(context, otherRolePriority, x => x.EditRoles);

		// Assert
		Assert.That(result, Is.False);
	}
}
