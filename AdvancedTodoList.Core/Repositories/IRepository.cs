using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications;

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
    /// Asynchronously retrieves an aggregate by applying a specification.
    /// </summary>
    /// <typeparam name="TDto">Type of the aggregate to retrieve.</typeparam>
    /// <param name="specification">Specification to apply.</param>
    /// <returns>
    /// A task that represents an asynchronous operation and the aggregate if found; 
    /// otherwise, <see langword="null"/>.
    /// </returns>
    Task<TDto?> GetAggregateAsync<TDto>(ISpecification<TEntity> specification) where TDto : class;

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
    Task<Page<TItem>> GetPageAsync<TItem>(PaginationParameters paginationParameters, ISpecification<TEntity> specification);

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
