namespace AdvancedTodoList.Core.Models;

/// <summary>
/// An interface that represents an entity with an owner ID property.
/// </summary>
public interface IHasOwner
{
	/// <summary>
	/// Foreign key referencing the user who created this entity.
	/// </summary>
	public string OwnerId { get; }
}
