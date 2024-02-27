using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

[TestFixture]
public class TodoListMembersRepositoryTests : BaseRepositoryTests<TodoListMember, int>
{
	private ITodoListMembersRepository MembersRepository => (ITodoListMembersRepository)Repository;

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

	[Test]
	public async Task FindAsync_MemberExists_ReturnsValidMember()
	{
		// Arrange
		var member = await AddTestEntityToDbAsync();

		// Act
		var result = await MembersRepository.FindAsync(member.TodoListId, member.UserId);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(result.TodoListId, Is.EqualTo(member.TodoListId));
			Assert.That(result.UserId, Is.EqualTo(member.UserId));
		});
	}

	[Test]
	public async Task FindAsync_MemberDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "wrong_todo_list";
		string userId = "no user :(";

		// Act
		var result = await MembersRepository.FindAsync(todoListId, userId);

		// Assert
		Assert.That(result, Is.Null);
	}
}
