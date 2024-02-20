using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Pagination;
using EUniversity.Core.Pagination;

namespace AdvancedTodoList.Core.Repositories;

/// <summary>
/// Represents a generic repository interface for CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of entity's primary key.</typeparam>
public interface IRepository<TEntity, TKey> 
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Asynchronously adds a new entity to the repository.
	/// </summary>
	/// <param name="entity">The entity to add.</param>
	Task AddAsync(TEntity entity);

	/// <summary>
	/// Asynchronously retrieves an entity by its primary key.
	/// </summary>
	/// <param name="id">The primary key of the entity to retrieve.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and the entity if found; 
	/// otherwise, <see langword="null"/>.
	/// </returns>
	Task<TEntity?> GetByIdAsync(TKey id);

	/// <summary>
	/// Asynchronously retrieves an entity by its primary key and maps it to a DTO.
	/// </summary>
	/// <typeparam name="TDto">The type of DTO to map the entity to.</typeparam>
	/// <param name="id">The primary key of the entity to retrieve.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and the mapped DTO if found;
	/// otherwise, <see langword="null"/>.
	/// </returns>
	Task<TDto?> GetByIdAndMapAsync<TDto>(TKey id);

	/// <summary>
	/// Asynchronously retrieves a page of DTOs based on pagination parameters.
	/// </summary>
	/// <typeparam name="TDto">The type of DTO to retrieve.</typeparam>
	/// <param name="parameters">The pagination parameters.</param>
	/// <returns>
	/// A task that represents an asynchronous operation and a page of DTOs.
	/// </returns>
	Task<Page<TDto>> GetPageAsync<TDto>(PaginationParameters parameters);

	/// <summary>
	/// Asynchronously updates an existing entity in the repository.
	/// </summary>
	/// <param name="entity">The entity to update.</param>
	Task UpdateAsync(TEntity entity);

	/// <summary>
	/// Asynchronously deletes an entity from the repository.
	/// </summary>
	/// <param name="entity">The entity to delete.</param>
	Task DeleteAsync(TEntity entity);
}
