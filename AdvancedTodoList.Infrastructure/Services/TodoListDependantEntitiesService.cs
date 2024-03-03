using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
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
	IRepository<TEntity, TKey> repository, IEntityExistenceChecker existenceChecker) :
	ITodoListDependantEntitiesService<TEntity, TKey>
	where TEntity : class, IEntity<TKey>, ITodoListDependant
	where TKey : IEquatable<TKey>
{
	private readonly IRepository<TEntity, TKey> _repository = repository;
	private readonly IEntityExistenceChecker _existenceChecker = existenceChecker;

	/// <summary>
	/// Retrieves a page of to-do list dependant entities mapped to <typeparamref name="TDto"/>.
	/// </summary>
	/// <remarks>
	/// This method checks if <paramref name="todoListId"/> is valid, but doesn't filter by it.
	/// Filtering should be done in <paramref name="specification"/>.
	/// </remarks>
	/// <param name="todoListId">The ID of the to-do list which items should be retrieved.</param>
	/// <param name="specification">Specification to apply to entities.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains a page of <see cref="TDto"/> objects or
	/// <see langword="null" /> if the to-do list does not exist.
	/// </returns>
	public async Task<Page<TDto>?> GetPageAsync<TDto>(string todoListId,
		ISpecification<TEntity> specification, PaginationParameters paginationParameters)
	{
		// Check if to-do list exists
		if (!await _existenceChecker.ExistsAsync<TodoList, string>(todoListId))
			return null;

		// Return the requested page
		return await _repository.GetPageAsync<TDto>(paginationParameters, specification);
	}

	/// <summary>
	/// Retrieves a to-do list dependant entity by its ID asynchronously and maps it to <typeparamref name="TDto"/>.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which entity will be retrieved.</param>
	/// <param name="entityId">The ID of the entity to retrieve.</param>
	/// <typeparam name="TDto">DTO to map entity to.</typeparam>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <typeparamref name="TDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public async Task<TDto?> GetByIdAsync<TDto>(string todoListId, TKey entityId)
		where TDto : class
	{
		// Get the model
		var entity = await _repository.GetByIdAsync(entityId);
		// Return null if model is null or has wrong to-do list ID
		if (entity == null || entity.TodoListId != todoListId) return null;
		// Map it to DTO and return
		return entity.Adapt<TDto>();
	}

	/// <summary>
	/// Creates a new a to-do list dependant entity asynchronously from the DTO.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list to associate the entity with.</param>
	/// <param name="dto">The DTO containing information for creating the entity.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <typeparamref name="TEntity"/> mapped to 
	/// <typeparamref name="TOutputDto"/> or <see langword="null" /> if to-do list with ID
	/// <paramref name="todoListId"/> does not exist.
	/// </returns>
	public async Task<TOutputDto?> CreateAsync<TInputDto, TOutputDto>(string todoListId, TInputDto dto)
		where TOutputDto : class
	{
		// Check if to-do list exists
		if (!await _existenceChecker.ExistsAsync<TodoList, string>(todoListId))
			return null;

		// Create the model
		var entity = dto.Adapt<TEntity>();
		// Set the foreign key
		entity.TodoListId = todoListId;
		// Save it
		await _repository.AddAsync(entity);
		// Map it to DTO and return
		return entity.Adapt<TOutputDto>();
	}

	/// <summary>
	/// Edits a to-do list dependant entity asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the entity.</param>
	/// <param name="entityId">The ID of the entity to edit.</param>
	/// <param name="dto">The DTO containing information for editing the entity.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity or the to-do list was not found.
	/// </returns>
	public async Task<bool> UpdateAsync<TDto>(string todoListId, TKey entityId, TDto dto)
	{
		// Get the model of a to-do list item
		var entity = await _repository.GetByIdAsync(entityId);
		// Check if it's valid
		if (entity == null || entity.TodoListId != todoListId)
			return false;

		// Update the model
		dto.Adapt(entity);
		// Save changes
		await _repository.UpdateAsync(entity);

		return true;
	}

	/// <summary>
	/// Deletes a to-do list dependant entity asynchronously.
	/// </summary>
	/// <param name="todoListId">The ID of the to-do list which contains the entity.</param>
	/// <param name="entityId">The ID of the entity to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if the entity was not found.
	/// </returns>
	public async Task<bool> DeleteAsync(string todoListId, TKey entityId)
	{
		// Get the model of a to-do list item
		var todoItem = await _repository.GetByIdAsync(entityId);
		// Check if it's valid
		if (todoItem == null || todoItem.TodoListId != todoListId)
			return false;

		// Delete the model
		await _repository.DeleteAsync(todoItem);

		return true;
	}
}
