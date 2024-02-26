using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

public class TodoListRolesRepositoryTests : BaseRepositoryTests<TodoListRole, int>
{
	protected override int NonExistingId => -1;

	private bool UpdatedHasSetStatePermission = true;
	private bool UpdatedHasAddMembersPermission = true;

	protected override void AssertUpdated(TodoListRole updatedEntity)
	{
		Assert.Multiple(() =>
		{
			Assert.That(updatedEntity.HasSetStatePermission, Is.EqualTo(UpdatedHasSetStatePermission));
			Assert.That(updatedEntity.HasAddMembersPermission, Is.EqualTo(UpdatedHasAddMembersPermission));
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
		entity.HasSetStatePermission = UpdatedHasSetStatePermission;
		entity.HasAddMembersPermission = UpdatedHasAddMembersPermission;
	}
}
