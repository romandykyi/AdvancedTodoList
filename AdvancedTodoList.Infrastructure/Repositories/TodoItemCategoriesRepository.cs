using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Infrastructure.Data;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on to-do items categories.
/// </summary>
public class TodoItemCategoriesRepository(ApplicationDbContext dbContext) :
	BaseRepository<TodoItemCategory, int>(dbContext)
{
}
