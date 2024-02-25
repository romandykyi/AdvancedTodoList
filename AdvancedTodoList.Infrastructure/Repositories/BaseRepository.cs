using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Data;
using AdvancedTodoList.Infrastructure.Pagination;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.Infrastructure.Repositories;

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
	/// Gets a page with entities mapped to type <typeparamref name="TItem"/> asynchronously.
	/// </summary>
	/// <typeparam name="TItem">Returned type of items on the page.</typeparam>
	/// <param name="paginationParameters">Pagination parameters.</param>
	/// <param name="specification">Specification used for entities retrival.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and a page with entities mapped
	/// to <typeparamref name="TItem"/> type.
	/// </returns>
	public async Task<Page<TItem>> GetPageAsync<TItem>(PaginationParameters paginationParameters, ISpecification<TEntity> specification)
	{
		return await DbContext.Set<TEntity>()
			.ApplySpecification(specification)
			.ToPageAsync<TEntity, TItem>(paginationParameters);
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
