namespace AdvancedTodoList.Core.Dtos;

public record TodoListItemView(int Id);

public record TodoListCreateDto(string Name);

public record TodoListGetByIdDto(string Id, string Name, TodoListItemView[] TodoItems);
