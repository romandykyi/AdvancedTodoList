using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Specifications;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do list items categories.
/// </summary>
public class TodoItemCategoriesService(
	ITodoListDependantEntitiesService<TodoItemCategory, int> helperService,
	IRepository<TodoItemCategory, int> categoriesRepository
	) : ITodoItemCategoriesService
{
	private readonly ITodoListDependantEntitiesService<TodoItemCategory, int> _helperService = helperService;
	private readonly IRepository<TodoItemCategory, int> _categoriesRepository = categoriesRepository;

	/// <summary>
	/// Asynchronously checks whether the category ID is valid for the given context.
	/// </summary>
	/// <remarks>
	/// <see langword="null" /> ID is considered as valid.
	/// </remarks>
	/// <param name="context">To-do list context.</param>
	/// <param name="categoryId">ID of the category to validate.</param>
	/// <returns>
	/// A task representing the asynchronous operation.
	/// <see langword="true" /> if ID is valid, <see langword="false" /> otherwise.
	/// </returns>
	public async Task<bool> IsCategoryValidForContext(TodoListContext context, int? categoryId)
	{
		// Return true, null category is allowed for any context
		if (categoryId == null) return true;

		var category = await _categoriesRepository.GetByIdAsync(categoryId.Value);
		return category != null && category.TodoListId == context.TodoListId;
	}

	/// <summary>
	/// Retrieves a page of to-do list items categories of the list with the specified ID.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="paginationParameters">Pagination parameters to use.</param>
	/// <param name="name">Optional name to filter categories by.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<Page<TodoItemCategoryViewDto>>> GetCategoriesOfListAsync(
		TodoListContext context, PaginationParameters paginationParameters, string? name = null)
	{
		TodoListDependantEntitiesSpecification<TodoItemCategory> specification = new(context.TodoListId, name);
		return _helperService.GetPageAsync<TodoItemCategoryViewDto>(context, specification, paginationParameters);
	}

	/// <summary>
	/// Retrieves a to-do list item category by its ID asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="categoryId">The ID of the to-do list item to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<TodoItemCategoryViewDto>> GetByIdAsync(TodoListContext context, int categoryId)
	{
		return _helperService.GetByIdAsync<TodoItemCategoryViewDto>(context, categoryId);
	}

	/// <summary>
	/// Creates a new to-do list item category asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="dto">The DTO containing information for creating the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponse<TodoItemCategoryViewDto>> CreateAsync(TodoListContext context, TodoItemCategoryCreateDto dto)
	{
		return _helperService.CreateAsync<TodoItemCategoryCreateDto, TodoItemCategoryViewDto>(context, dto, x => x.EditCategories);
	}

	/// <summary>
	/// Edits a to-do list item category asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="categoryId">The ID of the to-do list item to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list item.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>
	public Task<ServiceResponseStatus> EditAsync(TodoListContext context, int categoryId, TodoItemCategoryCreateDto dto)
	{
		return _helperService.UpdateAsync(context, categoryId, dto, x => x.EditCategories);
	}

	/// <summary>
	/// Deletes a to-do list item category asynchronously.
	/// </summary>
	/// <param name="context">To-do list context.</param>
	/// <param name="categoryId">The ID of the to-do list item to delete.</param>
	/// <returns>
	/// A task representing the asynchronous operation containing the result of operation.
	/// </returns>

	public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int categoryId)
	{
		return _helperService.DeleteAsync(context, categoryId, x => x.EditCategories);
	}
}
