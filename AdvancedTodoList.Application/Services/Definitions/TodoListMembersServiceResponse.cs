using AdvancedTodoList.Application.Dtos;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Class that represents possible results of the method <see cref="ITodoListMembersService.AddMemberAsync"/>.
/// </summary>
public class AddTodoListMemberServiceResult(
	TodoListMemberServiceResultStatus status, TodoListMemberMinimalViewDto? dto = null)
{
	/// <summary>
	/// Status of the operation.
	/// </summary>
	public TodoListMemberServiceResultStatus Status { get; } = status;

	/// <summary>
	/// Gets additional DTO of the member, can be <see langword="null" />.
	/// </summary>
	public TodoListMemberMinimalViewDto? Dto { get; } = dto;
}

/// <summary>
/// Enum that represents possible result statuses of the to-do list members service.
/// </summary>
public enum TodoListMemberServiceResultStatus
{
	/// <summary>
	/// Operation was successfull.
	/// </summary>
	Success,
	/// <summary>
	/// To-do list was not found.
	/// </summary>
	NotFound,
	/// <summary>
	/// User is already a member of the to-do list, returned only in the add member method.
	/// </summary>
	UserAlreadyAdded,
	/// <summary>
	/// Role either doesn't exist or it's invalid for the current to-do list.
	/// </summary>
	InvalidRoleId,
	/// <summary>
	/// User has no permission to perform the operation.
	/// </summary>
	Forbidden
}
