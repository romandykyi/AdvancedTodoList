using AdvancedTodoList.Core.Models.TodoLists.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTodoList.Core.Dtos;

/// <summary>
/// Represents a partial view for a role.
/// </summary>
public record TodoListRolePreviewDto(int Id, string Name);

/// <summary>
/// Represents a full view for a role.
/// </summary>
public record TodoListRoleViewDto(int Id, string Name, RolePermissions Permissions);

/// <summary>
/// DTO for creating/updating a role.
/// </summary>
public record TodoListRoleCreateDto(string Name, RolePermissions Permissions);