using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists items.
/// </summary>
public class TodoItemsService(
	ITodoListDependantEntitiesService<TodoItem, int> helperService,
	IRepository<TodoItem, int> repository,
	ITodoItemCategoriesService categoriesService,
	IPermissionsChecker permissionsChecker
	) : ITodoItemsService
{
	private readonly ITodoListDependantEntitiesService<TodoItem, int> _helperService = helperService;
	private readonly IRepository<TodoItem, int> _repository = repository;
	private readonly ITodoItemCategoriesService _categoriesService = categoriesService;
	private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;

	/// <summary>
	/// Retrieves a page of to-do list items of the list with the specified ID.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <param name="filter">Filter parameters to apply.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<Page<TodoItemPreviewDto>>> GetItemsOfListAsync(TodoListContext context,
		PaginationParameters paginationParameters, TodoItemsFilter filter)
	{
		TodoItemsSpecification specification = new(context.TodoListId, filter);
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

		// Return requested DTO
		return new(ServiceResponseStatus.Success, dto);
	}

	/// <summary>
	/// Creates a new to-do list item asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public async Task<TodoItemsServiceResponse> CreateAsync(TodoListContext context, TodoItemCreateDto dto)
	{
		// Validate the category
		if (!await _categoriesService.IsCategoryValidForContext(context, dto.CategoryId))
			return new(TodoItemsServiceStatus.InvalidCategoryId);

		var response = await _helperService.CreateAsync<TodoItemCreateDto, TodoItemGetByIdDto>
			(context, dto, x => x.AddItems);

		return new(ToTodoItemsServiceStatus(response.Status), response.Result);
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
	public async Task<TodoItemsServiceStatus> EditAsync(TodoListContext context, int itemId, TodoItemCreateDto dto)
	{
		// Validate the category
		if (!await _categoriesService.IsCategoryValidForContext(context, dto.CategoryId))
			return TodoItemsServiceStatus.InvalidCategoryId;

		var response = await _helperService.UpdateAsync(context, itemId, dto, x => x.EditItems);
		return ToTodoItemsServiceStatus(response);
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

	private static TodoItemsServiceStatus ToTodoItemsServiceStatus(ServiceResponseStatus status)
	{
		return status switch
		{
			ServiceResponseStatus.Success => TodoItemsServiceStatus.Success,
			ServiceResponseStatus.NotFound => TodoItemsServiceStatus.NotFound,
			ServiceResponseStatus.Forbidden => TodoItemsServiceStatus.Forbidden,
			_ => throw new ArgumentException("Invalid service response", nameof(status))
		};
	}
}
