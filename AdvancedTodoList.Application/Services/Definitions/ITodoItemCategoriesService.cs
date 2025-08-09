using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Interface for a service that manages to-do list items categories.
/// </summary>
public interface ITodoItemCategoriesService
{
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
    public Task<bool> IsCategoryValidForContext(TodoListContext context, int? categoryId);

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
        TodoListContext context, PaginationParameters paginationParameters, string? name = null);

    /// <summary>
    /// Retrieves a to-do list item category by its ID asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="categoryId">The ID of the to-do list item to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<TodoItemCategoryViewDto>> GetByIdAsync(TodoListContext context, int categoryId);

    /// <summary>
    /// Creates a new to-do list item category asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="dto">The DTO containing information for creating the to-do list item.</param>
    /// <param name="callerId">ID of the user who creates the to-do list item.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<TodoItemCategoryViewDto>> CreateAsync(TodoListContext context, TodoItemCategoryCreateDto dto);

    /// <summary>
    /// Edits a to-do list item category asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="categoryId">The ID of the to-do list item to edit.</param>
    /// <param name="dto">The DTO containing information for editing the to-do list item.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> EditAsync(TodoListContext context, int categoryId, TodoItemCategoryCreateDto dto);

    /// <summary>
    /// Deletes a to-do list item category asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="categoryId">The ID of the to-do list item to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int categoryId);
}
