using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Services.Repositories;
using AdvancedTodoList.Infrastructure.Data;

namespace AdvancedTodoList.Infrastructure.Services.Repositories;

/// <summary>
/// Represents an abstract generic repository for CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of entity's primary key.</typeparam>
public abstract class BaseRepository<TEntity, TKey>(ApplicationDbContext dbContext)
	: IRepository<TEntity, TKey>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	protected readonly ApplicationDbContext DbContext = dbContext;

	/// <summary>
	/// Asynchronously adds a new entity to the repository.
	/// </summary>
	/// <param name="entity">The entity to add.</param>
	public async Task AddAsync(TEntity entity)
	{
		DbContext.Add(entity);
		await DbContext.SaveChangesAsync();
	}

	/// <summary>
	/// Asynchronously retrieves an entity by its primary key.
	/// </summary>
	/// <param name="id">The primary key of the entity to retrieve.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and the entity if found; 
	/// otherwise, <see langword="null"/>.
	/// </returns>
	public async Task<TEntity?> GetByIdAsync(TKey id)
	{
		return await DbContext.Set<TEntity>().FindAsync(id);
	}

	/// <summary>
	/// Asynchronously updates an existing entity in the repository.
	/// </summary>
	/// <param name="entity">The entity to update.</param>
	public async Task UpdateAsync(TEntity entity)
	{
		DbContext.Update(entity);
		await DbContext.SaveChangesAsync();
	}

	/// <summary>
	/// Asynchronously deletes an entity from the repository.
	/// </summary>
	/// <param name="entity">The entity to delete.</param>
	public async Task DeleteAsync(TEntity entity)
	{
		DbContext.Remove(entity);
		await DbContext.SaveChangesAsync();
	}
}
