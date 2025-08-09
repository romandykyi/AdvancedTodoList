using AdvancedTodoList.Application.Dtos;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Represents possible results of the join to-do list by invitatation link operation.
/// </summary>
public class JoinByInvitationLinkResult(JoinByInvitationLinkStatus status, TodoListMemberMinimalViewDto? dto = null)
{
	/// <summary>
	/// Status of the operation.
	/// </summary>
	public JoinByInvitationLinkStatus Status { get; } = status;

	/// <summary>
	/// Gets additional DTO of the member, can be <see langword="null" />.
	/// </summary>
	public TodoListMemberMinimalViewDto? Dto { get; } = dto;
}

/// <summary>
/// Enum that represents possible result statuses of the join to-do list by invitatation link operation.
/// </summary>
public enum JoinByInvitationLinkStatus
{
	/// <summary>
	/// Operation was successfull.
	/// </summary>
	Success,
	/// <summary>
	/// Invitation link was not found.
	/// </summary>
	NotFound,
	/// <summary>
	/// Invitation link is expired.
	/// </summary>
	Expired,
	/// <summary>
	/// User is already a member of the to-do list.
	/// </summary>
	UserIsAlreadyMember
}