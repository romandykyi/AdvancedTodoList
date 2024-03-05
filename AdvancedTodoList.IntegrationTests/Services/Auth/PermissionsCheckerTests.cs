using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using NUnit.Framework.Interfaces;
using System.Data;

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

		// Act
		bool result = await _permissionsChecker.IsMemberOfListAsync(member.UserId, member.TodoListId);

		// Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public async Task IsMemberOfListAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var user = await CreateTestUserAsync();
		var list = await CreateTestTodoListAsync();

		// Act
		bool result = await _permissionsChecker.IsMemberOfListAsync(user.Id, list.Id);

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

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(user.Id, todoList.Id, x => x.AddMembers);

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

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(user.Id, todoList.Id, x => x.AddItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionAsync_UserHasNoRole_ReturnsFalse()
	{
		// Arrange
		var member = await CreateTestMemberAsync();

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(member.UserId, member.TodoListId, x => x.AddItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var user = await CreateTestUserAsync();
		var list = await CreateTestTodoListAsync();

		// Act
		bool result = await _permissionsChecker.HasPermissionAsync(user.Id, list.Id, x => x.AddItems);

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

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(user.Id, todoList.Id, testEntity, x => x.EditItems);

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

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(user.Id, todoList.Id, testEntity, x => x.EditItems);

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

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(user.Id, todoList.Id, testEntity, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task CanTouchEntityAsync_UserHasNoRole_ReturnsFalse()
	{
		// Arrange
		var member = await CreateTestMemberAsync();
		var testEntity = GetTestEntity("some-random-user-id");

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(member.UserId, member.TodoListId, testEntity, x => x.EditItems);

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

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(user.Id, todoList.Id, testEntity, x => x.EditItems);

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

		// Act
		bool result = await _permissionsChecker
			.CanTouchEntityAsync<TodoList, string>(user.Id, todoList.Id, testEntity, x => x.EditItems);

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
		var otherRole = await CreateTestRoleAsync(todoList.Id, 4, RolePermissions.All);
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(user.Id, todoList.Id, otherRole.Id, x => x.EditRoles);

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
		var otherRole = await CreateTestRoleAsync(todoList.Id, 4, RolePermissions.All);
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(user.Id, todoList.Id, otherRole.Id, x => x.EditRoles);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserHasNoRole_ReturnsFalse()
	{
		// Arrange
		var member = await CreateTestMemberAsync();
		var otherRole = await CreateTestRoleAsync(member.TodoListId, 4, RolePermissions.All);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(member.UserId, member.TodoListId, otherRole.Id, x => x.EditItems);

		// Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task HasPermissionOverRoleAsync_UserIsNotMember_ReturnsFalse()
	{
		// Arrange
		var todoList = await CreateTestTodoListAsync();
		var user = await CreateTestUserAsync();
		var otherRole = await CreateTestRoleAsync(todoList.Id, 4, RolePermissions.All);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(user.Id, todoList.Id, otherRole.Id, x => x.EditItems);

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
		var otherRole = await CreateTestRoleAsync(todoList.Id, 2, RolePermissions.All);
		var member = await CreateTestMemberAsync(user.Id, todoList.Id, role.Id);

		// Act
		bool result = await _permissionsChecker
			.HasPermissionOverRoleAsync(user.Id, todoList.Id, otherRole.Id, x => x.EditRoles);

		// Assert
		Assert.That(result, Is.False);
	}
}
