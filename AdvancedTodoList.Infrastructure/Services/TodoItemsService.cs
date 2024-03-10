using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Infrastructure.Specifications;
using Mapster;
using System.Security;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists items.
/// </summary>
public class TodoItemsService(
	ITodoListDependantEntitiesService<TodoItem, int> helperService,
	IRepository<TodoItem, int> repository,
	IEntityExistenceChecker existenceChecker,
	IPermissionsChecker permissionsChecker
	) : ITodoItemsService
{
	private readonly ITodoListDependantEntitiesService<TodoItem, int> _helperService = helperService;
	private readonly IRepository<TodoItem, int> _repository = repository;
	private readonly IEntityExistenceChecker _existenceChecker = existenceChecker;
	private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;

	/// <summary>
	/// Retrieves a page of to-do list items of the list with the specified ID.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<Page<TodoItemPreviewDto>>> GetItemsOfListAsync(TodoListContext context,
		PaginationParameters paginationParameters)
	{
		TodoListDependantEntitiesSpecification<TodoItem> specification = new(context.TodoListId);
		return _helperService.GetPageAsync<TodoItemPreviewDto>(context, specification, paginationParameters);
	}

	/// <summary>
	/// Retrieves a to-do list item by its ID asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="itemId">The ID of the to-do list item to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public async Task<ServiceResponse<TodoItemGetByIdDto>> GetByIdAsync(TodoListContext context, int itemId)
	{
		// Check is user is a member of the to-do list
		if (!await _permissionsChecker.IsMemberOfListAsync(context))
			return new(ServiceResponseStatus.Forbidden);

		TodoItemAggregateSpecification specification = new(itemId);
		// Get the aggregate
		var dto = await _repository.GetAggregateAsync<TodoItemGetByIdDto>(specification);
		// Check if it's valid
		if (dto == null || dto.TodoListId != context.TodoListId) 
			return new(ServiceResponseStatus.NotFound);

		// Retyrb requested DTO
		return new(ServiceResponseStatus.Success, dto);
	}

	/// <summary>
	/// Creates a new to-do list item asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list item.</param>
	/// <param name="callerId">ID of the user who creates the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<TodoItemGetByIdDto>> CreateAsync(TodoListContext context, TodoItemCreateDto dto, string callerId)
	{
		return _helperService.CreateAsync<TodoItemCreateDto, TodoItemGetByIdDto>(context, dto, x => x.AddItems);
	}

	/// <summary>
	/// Edits a to-do list item asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="itemId">The ID of the to-do list item to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> EditAsync(TodoListContext context, int itemId, TodoItemCreateDto dto)
	{
		return _helperService.UpdateAsync(context, itemId, dto, x => x.EditItems);
	}

	/// <summary>
	/// Updates the state of a to-do list item asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="itemId">The ID of the to-do list item to update the state.</param>
	/// <param name="stateDto">The DTO which contains the state of the to-do item to set.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> UpdateStateAsync(TodoListContext context, int itemId, TodoItemUpdateStateDto stateDto)
	{
		return _helperService.UpdateAsync(context, itemId, stateDto, x => x.SetItemsState);
	}

	/// <summary>
	/// Deletes a to-do list item asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="itemId">The ID of the to-do list item to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int itemId)
	{
		return _helperService.DeleteAsync(context, itemId, x => x.DeleteItems);
	}
}
