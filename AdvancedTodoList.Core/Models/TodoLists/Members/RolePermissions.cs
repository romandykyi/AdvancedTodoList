namespace AdvancedTodoList.Core.Models.TodoLists.Members;

/// <summary>
/// Represents permissions which to-do list members can have.
/// </summary>
/// <param name="SetItemsState">A flag that determines whether user can change a state of to-do list items (active/completed/skipped).</param>
/// <param name="AddItems">A flag that determines whether user can add to-do list items.</param>
/// <param name="EditItems">A flag that determines whether user can edit to-do list items of other users and the to-do list itself.</param>
/// <param name="DeleteItems"> A flag that determines whether user can delete to-do list items of other users.</param>
/// <param name="AddMembers">A flag that determines whether user can add members and create invitation links.</param>
/// <param name="RemoveMembers">A flag that determines whether user can remove members.</param>
/// <param name="AssignRoles">A flag that determines whether user can assign a role to other member.</param>
/// <param name="EditRoles">A flag that determines whether user can edit/delete existing roles and add new roles.</param>
/// <param name="EditCategories">A flag that determines whether user can edit/delete existing categories and add new categories.</param>
/// <param name="ManageInvitationLinks">A flag that determines whether user can view/delete existing invitation links.</param>
public record struct RolePermissions(
	bool SetItemsState = false,
	bool AddItems = false,
	bool EditItems = false,
	bool DeleteItems = false,
	bool AddMembers = false,
	bool RemoveMembers = false,
	bool AssignRoles = false,
	bool EditRoles = false,
	bool EditCategories = false,
	bool ManageInvitationLinks = false
	)
{
	/// <summary>
	/// Instance of a structure with all permissions.
	/// </summary>
	public static readonly RolePermissions All = new(true, true, true, true, true, true, true, true, true, true);
}
