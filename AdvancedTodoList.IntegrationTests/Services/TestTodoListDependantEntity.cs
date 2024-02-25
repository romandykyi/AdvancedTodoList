using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models;

namespace AdvancedTodoList.IntegrationTests.Services;

public class TestTodoListDependantEntity : IEntity<int>, ITodoListDependant
{
	public int Id { get; set; }
	public string TodoListId { get; set; } = null!;
	public string TestProperty { get; set; } = null!;
}
public record TestTodoListDependantViewDto(int Id, string TestProperty);
public record TestTodoListDependantCreateDto(string TestProperty);