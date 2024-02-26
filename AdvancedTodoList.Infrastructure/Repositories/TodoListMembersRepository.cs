using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Infrastructure.Data;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on to-do lists members.
/// </summary>
public class TodoListMembersRepository(ApplicationDbContext dbContext) :
	BaseRepository<TodoListMember, int>(dbContext)
{
}
