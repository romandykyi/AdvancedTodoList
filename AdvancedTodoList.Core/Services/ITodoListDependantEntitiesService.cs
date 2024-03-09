using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications;

namespace AdvancedTodoList.Core.Services;

/// <summary>
/// An interface that represents a service for perfoming CRUD operations
/// on to-do list dependant entities and mapping/unmapping DTOs.
/// </summary>
/// <typeparam name="TEntity">Type of the to-do list dependant entity.</typeparam>
/// <typeparam name="TKey">Type of the unique identifier used by the entity.</typeparam>
public interface ITodoListDependantEntitiesService<TEntity, TKey>
	where TEntity : class, IEntity<TKey>, ITodoListDependant
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Retrieves a page of to-do list dependant entities mapped to <typeparamref name="TDto"/>.
	/// </summary>
	/// <remarks>
	/// This method checks if to-do list ID is valid, but doesn't filter by it.
	/// Filtering should be done in <paramref name="specification"/>.
	/// </remarks>
	/// <param name="context">To-do list context.</param>
	/// <param name="specification">Specification to apply to entities.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation and the requested page on success.
	/// </returns>
	public Task<ServiceResponse<Page<TDto>>> GetPageAsync<TDto>(TodoListContext context,
		ISpecification<TEntity> specification, PaginationParameters paginationParameters);

	/// <summary>
	/// Retrieves a to-do list dependant entity by its ID asynchronously and maps it to <typeparamref name="TDto"/>.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="entityId">The ID of the entity to retrieve.</param>
	/// <typeparam name="TDto">DTO to map entity to.</typeparam>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation and the requested dto on success.
	/// </returns>
	Task<ServiceResponse<TDto>> GetByIdAsync<TDto>(TodoListContext context, TKey entityId) where TDto : class;

	/// <summary>
	/// Creates a new a to-do list dependant entity asynchronously from the DTO.
	/// </summary>
	/// <remarks>
	/// If <typeparamref name="TEntity"/> implements the <see cref="IHasOwner"/> interface, then
	/// this method will set the caller as an owner.
	/// </remarks>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for creating the entity.</param>
	/// <param name="permission">Accessor for the permission required for the user to perform the action.</param>
	/// <returns>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation and the created <typeparamref name="TEntity"/> mapped to 
	/// <typeparamref name="TOutputDto"/> on success.
	/// </returns>
	public Task<ServiceResponse<TOutputDto>> CreateAsync<TInputDto, TOutputDto>(
		TodoListContext context, TInputDto dto, Func<RolePermissions, bool> permission)
		where TOutputDto : class;

	/// <summary>
	/// Edits a to-do list dependant entity asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="entityId">The ID of the entity to edit.</param>
	/// <param name="dto">The DTO containing information for editing the entity.</param>
	/// <param name="permission">Accessor for the permission required for the user to perform the action.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponseStatus> UpdateAsync<TDto>(TodoListContext context, TKey entityId, 
		TDto dto, Func<RolePermissions, bool> permission);

	/// <summary>
	/// Deletes a to-do list dependant entity asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="entityId">The ID of the entity to delete.</param>
	/// <param name="permission">Accessor for the permission required for the user to perform the action.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context,
		TKey entityId, Func<RolePermissions, bool> permission);
}
