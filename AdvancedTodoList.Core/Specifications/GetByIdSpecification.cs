using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;

namespace AdvancedTodoList.Infrastructure.Specifications;

/// <summary>
/// Represents a specification that defines criteria for obtaining entities by ID.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity unique identifier.</typeparam>
/// <param name="id">The unique identifier to filter by.</param>
public class GetByIdSpecification<TEntity, TKey>(TKey id) : ISpecification<TEntity>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// The unique identifier to filter by.
	/// </summary>
	public TKey Id { get; } = id;

	/// <summary>
	/// Gets the criteria expression that defines the filtering conditions.
	/// Filters by ID provided in the constructor when not overriden.
	/// </summary>
	public Expression<Func<TEntity, bool>> Criteria => x => x.Id.Equals(Id);

	/// <summary>
	/// Gets the list of include expressions specifying related entities to be included in the query results.
	/// Is empty when not overriden.
	/// </summary>
	public virtual List<Expression<Func<TEntity, object?>>> Includes => [];

	/// <summary>
	/// Gets the list of include strings specifying related entities to be included in the query results.
	/// Is empty when not overriden.
	/// </summary>
	public virtual List<string> IncludeStrings => [];
}
