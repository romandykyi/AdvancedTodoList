namespace AdvancedTodoList.Core.Models.TodoLists;

/// <summary>
/// An interface which represents an entity which is dependant on a to-do list.
/// </summary>
public interface ITodoListDependant
{
    /// <summary>
    /// A foreign key of a to-do list.
    /// </summary>
    string TodoListId { get; set; }
}
