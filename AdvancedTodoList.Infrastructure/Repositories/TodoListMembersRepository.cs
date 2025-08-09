using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on to-do lists members.
/// </summary>
public class TodoListMembersRepository(ApplicationDbContext dbContext) :
    BaseRepository<TodoListMember, int>(dbContext), ITodoListMembersRepository
{
    /// <summary>
    /// Finds a to-do list member by to-do list ID and user's ID asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <param name="todoListId">ID of the to-do list.</param>
    /// <returns>
    /// Found to-do list member, or <see langword="null" /> if it was not found.
    /// </returns>
    public Task<TodoListMember?> FindAsync(string todoListId, string userId)
    {
        return DbContext.TodoListsMembers
            .Where(x => x.TodoListId == todoListId && x.UserId == userId)
            .FirstOrDefaultAsync();
    }
}
