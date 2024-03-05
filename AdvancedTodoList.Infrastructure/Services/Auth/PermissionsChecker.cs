using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTodoList.Infrastructure.Services.Auth;

/// <summary>
/// Service that checks user's permissions.
/// </summary>
public class PermissionsChecker(
	ITodoListMembersRepository membersRepository,
	IRepository<TodoListRole, int> rolesRepository) : IPermissionsChecker
{
	private readonly ITodoListMembersRepository _membersRepository = membersRepository;
	private readonly IRepository<TodoListRole, int> _rolesRepository = rolesRepository;

	/// <summary>
	/// Asynchronously checks whether the user is a member of the to-do list with
	/// specified ID.
	/// </summary>
	/// <param name="userId">ID of the user.</param>
	/// <param name="todoListId">ID of the to-do list.</param>
	/// <returns>
	/// <see langword="true" /> if user is a member of the list; otherwise <see langword="false" />.
	/// </returns>
	public async Task<bool> IsMemberOfListAsync(string userId, string todoListId)
	{
		return await _membersRepository.FindAsync(todoListId, userId) != null;
	}

	/// <summary>
	/// Asynchronously checks whether the user is a member of the to-do list and
	/// has a permission defined by the funciton <paramref name="permission"/>.
	/// </summary>
	/// <param name="userId">ID of the user.</param>
	/// <param name="todoListId">ID of the to-do list.</param>
	/// <param name="permission">Function that should return <see langword="true"/> if user has required permission.</param>
	/// <returns>
	/// <see langword="true" /> if user is a member of the list and has required permission; 
	/// otherwise <see langword="false" />.
	/// </returns>
	public async Task<bool> HasPermissionAsync(string userId, string todoListId, Func<RolePermissions, bool> permission)
	{
		var member = await _membersRepository.FindAsync(todoListId, userId);
		// User is not a member or has no role - return false
		if (member == null || member.RoleId == null) return false;

		// Check user's permissions
		var role = await _rolesRepository.GetByIdAsync(member.RoleId.Value);
		if (role == null) return false;

		return permission(role.Permissions);
	}

	/// <summary>
	/// Asynchronously checks whether the user can touch an entity.
	/// </summary>
	/// <remarks>
	/// This method firstly checks whether <paramref name="entity"/> implements <see cref="IHasOwner"/>
	/// interface and if yes, checks if the user is the owner of the entity and is a member of the to-do list; 
	/// otherwise the method checks if user has the permission defined by the function <paramref name="permission"/>.
	/// </remarks>
	/// <typeparam name="TEntity">Type of the entity.</typeparam>
	/// <typeparam name="TKey">Type of the unique identifier used by the entity.</typeparam>
	/// <param name="userId">ID of the user whose permissions are achecked.</param>
	/// <param name="todoListId">ID of the to-do list for which permission is checked.</param>
	/// <param name="entity">ID of the entity.</param>
	/// <param name="permission">Function that should return <see langword="true"/> if user has required permission.</param>
	/// <returns>
	/// <see langword="true"/> if user is either an owner of the entity and a member of a to-do list,
	/// or he/she/they has permission defined by <paramref name="permission"/>; otherwise <see langword="false" />.
	/// </returns>
	Task<bool> IPermissionsChecker.CanTouchEntityAsync<TEntity, TKey>(string userId, string todoListId, TEntity entity, Func<RolePermissions, bool> permission)
	{
		// If user owns entity only check if he/she/they is member
		if (entity is IHasOwner ownedEntity && ownedEntity.OwnerId == userId)
		{
			return IsMemberOfListAsync(userId, todoListId);
		}
		// Otherwise check if user has permission
		return HasPermissionAsync(userId, todoListId, permission);
	}

	/// <summary>
	/// Asynchronously checks whether the user is a member of the to-do list and
	/// has a permission defined by the funciton <paramref name="permission"/>.
	/// </summary>
	/// <param name="userId">ID of the user.</param>
	/// <param name="todoListId">ID of the to-do list.</param>
	/// <param name="permission">Function that should return <see langword="true"/> if user has required permission.</param>
	/// <returns>
	/// <see langword="true" /> if user is a member of the list and has required permission; 
	/// otherwise <see langword="false" />.
	/// </returns>
	public async Task<bool> HasPermissionOverRoleAsync(string userId, string todoListId, int roleId, Func<RolePermissions, bool> permission)
	{
		var member = await _membersRepository.FindAsync(todoListId, userId);
		// User is not a member or has no role - return false
		if (member == null || member.RoleId == null) return false;

		// Check if user has a permission
		var userRole = await _rolesRepository.GetByIdAsync(member.RoleId.Value);
		if (userRole == null || !permission(userRole.Permissions)) return false;

		// Get other role
		var role = await _rolesRepository.GetByIdAsync(roleId) ?? 
			throw new ArgumentException("Role with 'roleId' was not found", nameof(roleId));

		// Check if user has a higher priority
		return userRole.Priority < role.Priority;
	}
}
