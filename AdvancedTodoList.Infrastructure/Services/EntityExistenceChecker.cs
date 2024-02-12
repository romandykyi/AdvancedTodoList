using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that checks whether an entity with an ID exists.
/// </summary>
public class EntityExistenceChecker(ApplicationDbContext dbContext) : IEntityExistenceChecker
{
	private ApplicationDbContext _dbContext = dbContext;

	/// <summary>
	/// Asynchronously checks whether an entity of type <typeparamref name="TEntity"/> with an ID
	/// <paramref name="id"/> of type <typeparamref name="TId"/> exists.
	/// </summary>
	/// <typeparam name="TEntity">Type of the entity.</typeparam>
	/// <typeparam name="TId">Type which ID of the entity has.</typeparam>
	/// <param name="id">ID of the entity which existence is checked.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// <see langword="true"/> if entity with the given ID exists; otherwise
	/// <see langword="false"/>.
	/// </returns>
	public async Task<bool> ExistsAsync<TEntity, TId>(TId id)
		where TEntity : class, IEntity<TId>
		where TId : IEquatable<TId>
	{
		return await _dbContext.Set<TEntity>().AnyAsync(x => x.Id.Equals(id));
	}
}
