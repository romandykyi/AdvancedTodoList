using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications.Filters;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Interface for a service that manages to-do list items.
/// </summary>
public interface ITodoItemsService
{
    /// <summary>
    /// Retrieves a page of to-do list items of the list with the specified ID.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="paginationParameters">Pagination parameters to use.</param>
    /// <param name="filter">Filter parameters to apply.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<Page<TodoItemPreviewDto>>> GetItemsOfListAsync(
        TodoListContext context, PaginationParameters paginationParameters, TodoItemsFilter filter);

    /// <summary>
    /// Retrieves a to-do list item by its ID asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="itemId">The ID of the to-do list item to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<TodoItemGetByIdDto>> GetByIdAsync(TodoListContext context, int itemId);

    /// <summary>
    /// Creates a new to-do list item asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="dto">The DTO containing information for creating the to-do list item.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<TodoItemsServiceResponse> CreateAsync(TodoListContext context, TodoItemCreateDto dto);

    /// <summary>
    /// Edits a to-do list item asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="itemId">The ID of the to-do list item to edit.</param>
    /// <param name="dto">The DTO containing information for editing the to-do list item.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<TodoItemsServiceStatus> EditAsync(TodoListContext context, int itemId, TodoItemCreateDto dto);

    /// <summary>
    /// Updates the state of a to-do list item asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="itemId">The ID of the to-do list item to update the state.</param>
    /// <param name="stateDto">The DTO which contains the state of the to-do item to set.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> UpdateStateAsync(TodoListContext context, int itemId, TodoItemUpdateStateDto stateDto);

    /// <summary>
    /// Deletes a to-do list item asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="itemId">The ID of the to-do list item to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int itemId);
}
