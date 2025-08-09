using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;

namespace AdvancedTodoList.IntegrationTests.Services;

public class TestTodoListDependantEntity : IEntity<int>, ITodoListDependant, IHasOwner
{
    public int Id { get; set; }
    public string TodoListId { get; set; } = null!;
    public string TestProperty { get; set; } = null!;
    public string? OwnerId { get; set; }
}
public record TestTodoListDependantViewDto(int Id, string TestProperty);
public record TestTodoListDependantCreateDto(string TestProperty);