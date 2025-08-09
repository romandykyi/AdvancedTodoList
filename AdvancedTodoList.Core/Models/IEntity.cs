namespace AdvancedTodoList.Core.Models;

/// <summary>
/// An interface that represents an entity with an ID property.
/// </summary>
/// <typeparam name="TId">Type of the entity ID.</typeparam>
public interface IEntity<TId> where TId : IEquatable<TId>
{
    TId Id { get; }
}
