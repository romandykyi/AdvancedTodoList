using AdvancedTodoList.Core.Dtos;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// Class that represents possible results of the method <see cref="ITodoListMembersService.AddMemberAsync(string, AdvancedTodoList.Core.Dtos.TodoListMemberAddDto)"/>.
/// </summary>
public class AddTodoListMemberResult(
	AddTodoListMemberResultStatus status, TodoListMemberMinimalViewDto? dto = null)
{
	/// <summary>
	/// Status of the operation.
	/// </summary>
	public AddTodoListMemberResultStatus Status { get; } = status;

	/// <summary>
	/// Gets DTO of added to-do list member or <see langword="null" /> if
	/// status does not indicate success.
	/// </summary>
	public TodoListMemberMinimalViewDto? Dto { get; } = dto;
}

/// <summary>
/// Enum that represents possible result types of the method<see cref="ITodoListMembersService.AddMemberAsync(string, AdvancedTodoList.Core.Dtos.TodoListMemberAddDto)"/>.
/// </summary>
public enum AddTodoListMemberResultStatus
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
	/// User is already a member of the to-do list.
	/// </summary>
	UserAlreadyAdded
}
