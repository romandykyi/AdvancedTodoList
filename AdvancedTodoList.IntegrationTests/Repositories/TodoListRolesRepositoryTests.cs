using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

public class TodoListRolesRepositoryTests : BaseRepositoryTests<TodoListRole, int>
{
	protected override int NonExistingId => -1;

	private readonly RolePermissions UpdatedPermissions = new(AddMembers: true, SetItemsState: true);

	protected override void AssertUpdated(TodoListRole updatedEntity)
	{
		Assert.Multiple(() =>
		{
			Assert.That(updatedEntity.Permissions, Is.EqualTo(UpdatedPermissions));
		});
	}

	protected override async Task<TodoListRole> CreateTestEntityAsync()
	{
		var user = TestModels.CreateTestUser();
		DbContext.Add(user);
		var todoList = TestModels.CreateTestTodoList();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();

		return TestModels.CreateTestRole(todoList.Id);
	}

	protected override void UpdateEntity(TodoListRole entity)
	{
		entity.Permissions = UpdatedPermissions;
	}
}
