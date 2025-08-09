using AdvancedTodoList.Core.Models.TodoLists.Members;

namespace AdvancedTodoList.Application.Dtos;

/// <summary>
/// Represents a partial view for a role.
/// </summary>
public record TodoListRolePreviewDto(int Id, string Name);

/// <summary>
/// Represents a full view for a role.
/// </summary>
public record TodoListRoleViewDto(int Id, string Name, int Priority, RolePermissions Permissions);

/// <summary>
/// DTO for creating/updating a role.
/// </summary>
public record TodoListRoleCreateDto(string Name, int Priority, RolePermissions Permissions);