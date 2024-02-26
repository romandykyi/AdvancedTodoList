using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

[TestFixture]
public class TodoListMembersRepositoryTests : BaseRepositoryTests<TodoListMember, int>
{
	protected override int NonExistingId => -1;

	private readonly int? UpdatedRoleId = null;

	protected override void AssertUpdated(TodoListMember updatedEntity)
	{
		Assert.That(updatedEntity.RoleId, Is.EqualTo(UpdatedRoleId));
	}

	protected override async Task<TodoListMember> CreateTestEntityAsync()
	{
		var user = TestModels.CreateTestUser();
		DbContext.Add(user);
		var todoList = TestModels.CreateTestTodoList();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		var role = TestModels.CreateTestRole(todoList.Id);
		DbContext.Add(role);
		await DbContext.SaveChangesAsync();

		return new()
		{
			TodoListId = todoList.Id,
			UserId = user.Id,
			RoleId = role.Id
		};
	}

	protected override void UpdateEntity(TodoListMember entity)
	{
		entity.RoleId = UpdatedRoleId;
	}
}
