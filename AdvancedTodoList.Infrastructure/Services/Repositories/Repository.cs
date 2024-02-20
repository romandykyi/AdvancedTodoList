using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Data;
using EUniversity.Core.Pagination;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Services.Repositories;

/// <summary>
/// Represents a generic repository for CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of entity's primary key.</typeparam>
public class Repository<TEntity, TKey>(ApplicationDbContext dbContext)
	: IRepository<TEntity, TKey>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	private readonly ApplicationDbContext _dbContext = dbContext;

	/// <summary>
	/// Asynchronously adds a new entity to the repository.
	/// </summary>
	/// <param name="entity">The entity to add.</param>
	public async Task AddAsync(TEntity entity)
	{
		_dbContext.Add(entity);
		await _dbContext.SaveChangesAsync();
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
		return await _dbContext.Set<TEntity>().FindAsync(id);
	}

	/// <summary>
	/// Asynchronously retrieves an entity by its primary key and maps it to a DTO.
	/// </summary>
	/// <typeparam name="TDto">The type of DTO to map the entity to.</typeparam>
	/// <param name="id">The primary key of the entity to retrieve.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and the mapped DTO if found;
	/// otherwise, <see langword="null"/>.
	/// </returns>
	public async Task<TDto?> GetByIdAndMapAsync<TDto>(TKey id)
	{
		return await _dbContext.Set<TEntity>()
			.AsNoTracking()
			.Where(x => x.Id.Equals(id))
			.ProjectToType<TDto>()
			.FirstOrDefaultAsync();
	}

	/// <summary>
	/// Asynchronously retrieves a page of DTOs based on pagination parameters.
	/// </summary>
	/// <typeparam name="TDto">The type of DTO to retrieve.</typeparam>
	/// <param name="parameters">The pagination parameters.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and a page of DTOs.
	/// </returns>
	public Task<Page<TDto>> GetPageAsync<TDto>(PaginationParameters parameters)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Asynchronously updates an existing entity in the repository.
	/// </summary>
	/// <param name="entity">The entity to update.</param>
	public async Task UpdateAsync(TEntity entity)
	{
		_dbContext.Update(entity);
		await _dbContext.SaveChangesAsync();
	}

	/// <summary>
	/// Asynchronously deletes an entity from the repository.
	/// </summary>
	/// <param name="entity">The entity to delete.</param>
	public async Task DeleteAsync(TEntity entity)
	{
		_dbContext.Remove(entity);
		await _dbContext.SaveChangesAsync();
	}
}
