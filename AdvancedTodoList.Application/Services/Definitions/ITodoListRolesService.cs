using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Interface for a service that manages to-do list role.
/// </summary>
public interface ITodoListRolesService
{
    /// <summary>
    /// Retrieves a page of to-do list roles of the list with the specified ID.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="paginationParameters">Pagination parameters to use.</param>
    /// <param name="name">Optional name to filter categories by.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<Page<TodoListRolePreviewDto>>> GetRolesOfListAsync(
        TodoListContext context, PaginationParameters paginationParameters, string? name = null);

    /// <summary>
    /// Retrieves a to-do list role by its ID asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="roleId">The ID of the to-do list role to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<TodoListRoleViewDto>> GetByIdAsync(TodoListContext context, int roleId);

    /// <summary>
    /// Creates a new to-do list role asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="dto">The DTO containing information for creating the to-do list role.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<TodoListRoleViewDto>> CreateAsync(TodoListContext context, TodoListRoleCreateDto dto);

    /// <summary>
    /// Edits a to-do list role asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="roleId">The ID of the to-do list role to edit.</param>
    /// <param name="dto">The DTO containing information for editing the to-do list role.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> EditAsync(TodoListContext context, int roleId, TodoListRoleCreateDto dto);

    /// <summary>
    /// Deletes a to-do list role asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="roleId">The ID of the to-do list role to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int roleId);
}
