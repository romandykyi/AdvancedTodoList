using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Infrastructure.Data;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on a to-do items.
/// </summary>
public class TodoItemsRepository(ApplicationDbContext dbContext) :
	BaseRepository<TodoItem, int>(dbContext)
{
}
