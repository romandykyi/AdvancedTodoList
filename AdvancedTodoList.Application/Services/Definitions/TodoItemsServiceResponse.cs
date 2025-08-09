using AdvancedTodoList.Application.Dtos;

namespace AdvancedTodoList.Application.Services.Definitions;

public class TodoItemsServiceResponse(TodoItemsServiceStatus status, TodoItemGetByIdDto? result = null)
{
    /// <summary>
    /// Status of the operation.
    /// </summary>
    public TodoItemsServiceStatus Status { get; set; } = status;

    /// <summary>
    /// Result of the operation.
    /// </summary>
    public TodoItemGetByIdDto? Result { get; set; } = result;
}

public enum TodoItemsServiceStatus
{
    /// <summary>
    /// Status that indicates success.
    /// </summary>
    Success,
    /// <summary>
    /// Status that indicates that entity could not be found or the caller isn't
    /// suppossed to know that entity exists.
    /// </summary>
    NotFound,
    /// <summary>
    /// Status that indicates that user has no permission to perform the operation.
    /// </summary>
    Forbidden,
    /// <summary>
    /// Status that indicates that user provided an invalid category ID.
    /// </summary>
    InvalidCategoryId
}