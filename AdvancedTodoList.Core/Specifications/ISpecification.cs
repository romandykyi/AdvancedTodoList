using System.Linq.Expressions;

namespace AdvancedTodoList.Core.Specifications;

/// <summary>
/// Represents a specification that defines criteria for filtering entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the criteria expression that defines the filtering conditions.
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Gets the list of include expressions specifying related entities to be included in the query results.
    /// </summary>
    List<Expression<Func<T, object?>>> Includes { get; }

    /// <summary>
    /// Gets the list of include strings specifying related entities to be included in the query results.
    /// </summary>
    List<string> IncludeStrings { get; }
}
