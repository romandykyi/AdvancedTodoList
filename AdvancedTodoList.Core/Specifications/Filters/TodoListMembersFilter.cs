namespace AdvancedTodoList.Core.Specifications.Filters;

/// <summary>
/// Parameters for filtering to-do lists members.
/// </summary>
/// <param name="RoleId">IDs of the roles to filter by.</param>
/// <param name="UserId">Optional ID of the user to filter by.</param>
/// <param name="UserName">
/// Optional username to filter by. 
/// Entries which have this substring in the username will be returned.
/// </param>
/// <param name="FullName">
/// Optional full name to filter by. 
/// Entries which have this substring in the '{FirstName} {LastName}' will be returned.
/// </param>
public record TodoListMembersFilter(IEnumerable<int?>? RoleId = null, string? UserId = null,
	string? UserName = null, string? FullName = null);
