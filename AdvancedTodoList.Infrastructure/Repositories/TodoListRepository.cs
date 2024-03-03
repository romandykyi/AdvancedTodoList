using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Infrastructure.Data;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on to-do lists.
/// </summary>
public class TodoListRepository(ApplicationDbContext dbContext) :
	BaseRepository<TodoList, string>(dbContext)
{
}
