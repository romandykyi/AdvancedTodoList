using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Infrastructure.Data;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on to-do lists members roles.
/// </summary>
public class TodoListMembersRolesRepository(ApplicationDbContext dbContext) :
	BaseRepository<TodoListMemberRole, int>(dbContext)
{
}
