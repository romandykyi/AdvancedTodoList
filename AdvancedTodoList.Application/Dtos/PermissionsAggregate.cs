using AdvancedTodoList.Core.Models.TodoLists.Members;

namespace AdvancedTodoList.Application.Dtos;

/// <summary>
/// Represents an aggregate of a to-do list member with a role.
/// </summary>
public record PermissionsAggregate(RoleEssentials? Role);

/// <summary>
/// Represents role's permissions and priority.
/// </summary>
public record RoleEssentials(int Priority, RolePermissions Permissions);
