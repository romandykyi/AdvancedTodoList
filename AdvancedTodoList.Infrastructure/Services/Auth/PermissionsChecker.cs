using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Infrastructure.Specifications;

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
	/// <param name="context">To-do list context.</param>
	/// <returns>
	/// <see langword="true" /> if user is a member of the list; otherwise <see langword="false" />.
	/// </returns>
	public async Task<bool> IsMemberOfListAsync(TodoListContext context)
	{
		return await _membersRepository.FindAsync(context.TodoListId, context.CallerId) != null;
	}

	/// <summary>
	/// Asynchronously checks whether the user is a member of the to-do list and
	/// has a permission defined by the funciton <paramref name="permission"/>.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="permission">Function that should return <see langword="true"/> if user has required permission.</param>
	/// <returns>
	/// <see langword="true" /> if user is a member of the list and has required permission; 
	/// otherwise <see langword="false" />.
	/// </returns>
	public async Task<bool> HasPermissionAsync(TodoListContext context, Func<RolePermissions, bool> permission)
	{
		MemberPermissionsSpecification specification = new(context.TodoListId, context.CallerId);
		var member = await _membersRepository.GetAggregateAsync<PermissionsAggregate>(specification);
		// User is not a member or has no role - return false
		if (member == null || member.Role == null) return false;

		return permission(member.Role.Permissions);
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
	/// <param name="context">To-do list context.</param>
	/// <param name="entity">ID of the entity.</param>
	/// <param name="permission">Function that should return <see langword="true"/> if user has required permission.</param>
	/// <returns>
	/// <see langword="true"/> if user is either an owner of the entity and a member of a to-do list,
	/// or he/she/they has permission defined by <paramref name="permission"/>; otherwise <see langword="false" />.
	/// </returns>
	Task<bool> IPermissionsChecker.CanTouchEntityAsync<TEntity, TKey>(TodoListContext context, TEntity entity, Func<RolePermissions, bool> permission)
	{
		// If user owns entity only check if he/she/they is member
		if (entity is IHasOwner ownedEntity && ownedEntity.OwnerId == context.CallerId)
		{
			return IsMemberOfListAsync(context);
		}
		// Otherwise check if user has permission
		return HasPermissionAsync(context, permission);
	}

	/// <summary>
	/// Asynchronously checks whether the user has a permission to change the role
	/// defined by <paramref name="roleId"/>.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="roleId">ID of the role.</param>
	/// <param name="permission">Function that should return <see langword="true"/> if user has required permission.</param>
	/// <returns>
	/// <see langword="true"/> if user has <paramref name="permission"/> and highest role priority than
	/// the role defined by <paramref name="roleId"/>; otherwise <see langword="false" />.
	/// </returns>
	public async Task<bool> HasPermissionOverRoleAsync(TodoListContext context, int roleId, Func<RolePermissions, bool> permission)
	{
		MemberPermissionsSpecification specification = new(context.TodoListId, context.CallerId);
		var member = await _membersRepository.GetAggregateAsync<PermissionsAggregate>(specification);

		// User is not a member, has no role or permission - return false
		if (member == null || member.Role == null || !permission(member.Role.Permissions)) 
			return false;

		// Get other role
		var role = await _rolesRepository.GetByIdAsync(roleId) ??
			throw new ArgumentException("Role with 'roleId' was not found", nameof(roleId));

		// Check if user has a higher priority
		return member.Role.Priority < role.Priority;
	}
}
