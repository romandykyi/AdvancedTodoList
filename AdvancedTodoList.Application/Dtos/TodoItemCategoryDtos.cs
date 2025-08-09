namespace AdvancedTodoList.Core.Dtos;

public record TodoItemCategoryCreateDto(string Name);

public record TodoItemCategoryViewDto(int Id, string Name);