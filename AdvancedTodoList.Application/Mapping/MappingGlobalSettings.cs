using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using Mapster;

namespace AdvancedTodoList.Application.Mapping;

/// <summary>
/// Class that defines global mapping settings.
/// </summary>
public static class MappingGlobalSettings
{
    /// <summary>
    /// Apply global mapping settings.
    /// </summary>
    public static void Apply()
    {
        // Ignore null IDs
        TypeAdapterConfig<TodoListMember, TodoListMemberPreviewDto>.NewConfig()
            .IgnoreIf((src, dest) => src.RoleId == null, dest => dest.Role!);
        TypeAdapterConfig<TodoItem, TodoItemGetByIdDto>.NewConfig()
            .IgnoreIf((src, dest) => src.CategoryId == null, dest => dest.Category!);
        TypeAdapterConfig<TodoItem, TodoItemPreviewDto>.NewConfig()
            .IgnoreIf((src, dest) => src.CategoryId == null, dest => dest.Category!);

        // Convert null strings into empty strings and trim strings
        TypeAdapterConfig.GlobalSettings.Default
            .AddDestinationTransform((string? dest) => dest != null ? dest.Trim() : string.Empty);
    }
}
