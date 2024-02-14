using AdvancedTodoList.Core.Models;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// An interface for the service that checks whether an entity with an ID exists.
/// </summary>
public interface IEntityExistenceChecker
{
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
	Task<bool> ExistsAsync<TEntity, TId>(TId id)
		where TEntity : class, IEntity<TId>
		where TId : IEquatable<TId>;
}
