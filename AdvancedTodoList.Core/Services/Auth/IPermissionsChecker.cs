using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists.Members;

namespace AdvancedTodoList.Core.Services.Auth;

/// <summary>
/// Interface for a service that checks user's permissions.
/// </summary>
public interface IPermissionsChecker
{
	/// <summary>
	/// Asynchronously checks whether the user is a member of the to-do list with
	/// specified ID.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <returns>
	/// <see langword="true" /> if user is a member of the list; otherwise <see langword="false" />.
	/// </returns>
	Task<bool> IsMemberOfListAsync(TodoListContext context);

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
	Task<bool> HasPermissionAsync(TodoListContext context, Func<RolePermissions, bool> permission);

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
	Task<bool> CanTouchEntityAsync<TEntity, TKey>(TodoListContext context, TEntity entity, 
		Func<RolePermissions, bool> permission)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>;

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
	Task<bool> HasPermissionOverRoleAsync(TodoListContext context, int roleId, 
		Func<RolePermissions, bool> permission);
}
