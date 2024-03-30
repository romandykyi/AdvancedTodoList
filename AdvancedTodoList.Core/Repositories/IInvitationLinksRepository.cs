using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.Core.Repositories;

public interface IInvitationLinksRepository : IRepository<InvitationLink, int>
{
	/// <summary>
	/// Finds an invintation link by its value asynchronously.
	/// </summary>
	/// <param name="linkValue">Value of the link.</param>
	/// <returns>
	/// A task representing asynchronous operation which contains requested link or
	/// <see cref="null" /> it was not found.
	/// </returns>
	Task<InvitationLink?> FindAsync(string linkValue);
}
