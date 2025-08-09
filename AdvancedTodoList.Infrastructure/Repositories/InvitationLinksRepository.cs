using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on invitation links.
/// </summary>
public class InvitationLinksRepository(ApplicationDbContext dbContext) :
    BaseRepository<InvitationLink, int>(dbContext), IInvitationLinksRepository
{
    /// <summary>
    /// Finds an invintation link by its value asynchronously.
    /// </summary>
    /// <param name="linkValue">Value of the link.</param>
    /// <returns>
    /// A task representing asynchronous operation which contains requested link.
    /// </returns>
    public Task<InvitationLink?> FindAsync(string linkValue)
    {
        return DbContext.InvitationLinks
            .Where(x => x.Value == linkValue)
            .FirstOrDefaultAsync();
    }
}
