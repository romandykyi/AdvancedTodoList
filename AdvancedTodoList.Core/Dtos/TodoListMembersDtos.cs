﻿namespace AdvancedTodoList.Core.Dtos;

/// <summary>
/// DTO for a minimal view of a to-do list.
/// </summary>
public record TodoListMemberMinimalView(int Id, string UserId, string TodoListId, int? RoleId);

/// <summary>
/// DTO to view a member and their role.
/// </summary>
public record TodoListMemberPreviewDto(ApplicationUserPreviewDto Member, TodoListRolePreviewDto Role);

/// <summary>
/// DTO for adding a member to a to-do list.
/// </summary>
public record TodoListMemberAddDto(string UserId, int? RoleId = null);

/// <summary>
/// DTO for updating to-do list member's role.
/// </summary>
public record TodoListMemberUpdateRoleDto(int? RoleId);
