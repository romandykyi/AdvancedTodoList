namespace AdvancedTodoList.Core.Models;

/// <summary>
/// Represents an interface for an entity which has a string property representing
/// a name.
/// </summary>
public interface IHasName
{
	public string Name { get; }
}
