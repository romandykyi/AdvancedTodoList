using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Core.Specifications;
using Mapster;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that performs CRUD operations
/// on to-do list dependant entities and maps/unmaps DTOs.
/// </summary>
/// <typeparam name="TEntity">Type of the to-do list dependant entity.</typeparam>
/// <typeparam name="TKey">Type of the unique identifier used by the entity.</typeparam>
public sealed class TodoListDependantEntitiesService<TEntity, TKey>(
	IRepository<TEntity, TKey> repository, 
	IEntityExistenceChecker existenceChecker,
	IPermissionsChecker permissionsChecker) : ITodoListDependantEntitiesService<TEntity, TKey>
	where TEntity : class, IEntity<TKey>, ITodoListDependant
	where TKey : IEquatable<TKey>
{
	private readonly IRepository<TEntity, TKey> _repository = repository;
	private readonly IEntityExistenceChecker _existenceChecker = existenceChecker;
	private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;	

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
	public async Task<ServiceResponse<Page<TDto>>> GetPageAsync<TDto>(TodoListContext context,
		ISpecification<TEntity> specification, PaginationParameters paginationParameters)
	{
		// Check if to-do list exists
		if (!await _existenceChecker.ExistsAsync<TodoList, string>(context.TodoListId))
			return new(ServiceResponseStatus.NotFound);

		// Check if user is a member of the to-do list
		if (!await _permissionsChecker.IsMemberOfListAsync(context))
			return new(ServiceResponseStatus.Forbidden);

		// Get the requested page
		var page = await _repository.GetPageAsync<TDto>(paginationParameters, specification);
		// Return the page
		return new(ServiceResponseStatus.Success, page);
	}

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
	public async Task<ServiceResponse<TDto>> GetByIdAsync<TDto>(TodoListContext context, TKey entityId)
		where TDto : class
	{
		// Check if user is a member of the to-do list
		if (!await _permissionsChecker.IsMemberOfListAsync(context))
			return new(ServiceResponseStatus.Forbidden);

		// Get the model
		var entity = await _repository.GetByIdAsync(entityId);
		// Return null if model is null or has wrong to-do list ID
		if (entity == null || entity.TodoListId != context.TodoListId) 
			return new(ServiceResponseStatus.NotFound);

		// Map it to DTO and return
		var result = entity.Adapt<TDto>();
		return new(ServiceResponseStatus.Success, result);
	}

	/// <summary>
	/// Creates a new a to-do list dependant entity asynchronously from the DTO.
	/// </summary>
	/// <remarks>
	/// If <typeparamref name="TEntity"/> implements the <see cref="IHasOwner"/> interface, then
	/// this method will set the caller as an owner.
	/// </remarks>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for creating the entity.</param>
	/// <param name="permission">Optional accessor for the permission required for the user to perform the action.</param>
	/// <returns>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation and the created <typeparamref name="TEntity"/> mapped to 
	/// <typeparamref name="TOutputDto"/> on success.
	/// </returns>
	public async Task<ServiceResponse<TOutputDto>> CreateAsync<TInputDto, TOutputDto>(
		TodoListContext context, TInputDto dto, Func<RolePermissions, bool>? permission = null)
		where TOutputDto : class
	{
		// Check if to-do list exists
		if (!await _existenceChecker.ExistsAsync<TodoList, string>(context.TodoListId))
			return new(ServiceResponseStatus.NotFound);
		// Check if the user has the permission
		if (permission != null && !await _permissionsChecker.HasPermissionAsync(context, permission))
			return new(ServiceResponseStatus.Forbidden);

		// Create the model
		var entity = dto.Adapt<TEntity>();
		// Set the foreign key
		entity.TodoListId = context.TodoListId;
		// Set the owner
		if (entity is IHasOwner hasOwnerEntity)
			hasOwnerEntity.OwnerId = context.CallerId;
		// Save it
		await _repository.AddAsync(entity);
		// Map it to DTO and return
		var result = entity.Adapt<TOutputDto>();
		return new(ServiceResponseStatus.Success, result);
	}

	/// <summary>
	/// Edits a to-do list dependant entity asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="entityId">The ID of the entity to edit.</param>
	/// <param name="dto">The DTO containing information for editing the entity.</param>
	/// <param name="permission">Optional accessor for the permission required for the user to perform the action.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public async Task<ServiceResponseStatus> UpdateAsync<TDto>(
		TodoListContext context, TKey entityId, TDto dto, Func<RolePermissions, bool>? permission = null)
	{
		// Get the model of a to-do list item
		var entity = await _repository.GetByIdAsync(entityId);
		// Check if it's valid
		if (entity == null || entity.TodoListId != context.TodoListId)
			return ServiceResponseStatus.NotFound;

		// Check if user has the permission
		if (permission != null && !await _permissionsChecker.CanTouchEntityAsync<TEntity, TKey>(context, entity, permission))
			return ServiceResponseStatus.Forbidden;

		// Update the model
		dto.Adapt(entity);
		// Save changes
		await _repository.UpdateAsync(entity);

		return ServiceResponseStatus.Success;
	}

	/// <summary>
	/// Deletes a to-do list dependant entity asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="entityId">The ID of the entity to delete.</param>
	/// <param name="permission">Optional accessor for the permission required for the user to perform the action.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task contains
	/// a result of the operation.
	/// </returns>
	public async Task<ServiceResponseStatus> DeleteAsync(
		TodoListContext context, TKey entityId, Func<RolePermissions, bool>? permission = null)
	{
		// Get the model of a to-do list item
		var todoItem = await _repository.GetByIdAsync(entityId);
		// Check if it's valid
		if (todoItem == null || todoItem.TodoListId != context.TodoListId)
			return ServiceResponseStatus.NotFound;
		// Check if user has the permission
		if (permission != null && !await _permissionsChecker.CanTouchEntityAsync<TEntity, TKey>(context, todoItem, permission))
			return ServiceResponseStatus.Forbidden;

		// Delete the model
		await _repository.DeleteAsync(todoItem);

		return ServiceResponseStatus.Success;
	}
}
