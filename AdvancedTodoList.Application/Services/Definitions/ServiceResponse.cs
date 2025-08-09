namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// Represent a generic service response which indicates an operation status and
/// contains an additonal information.
/// </summary>
/// <typeparam name="T">Type of the content saved in the response.</typeparam>
public class ServiceResponse<T>(ServiceResponseStatus status, T? result = default)
{
    /// <summary>
    /// Gets the status of operation.
    /// </summary>
    public ServiceResponseStatus Status { get; } = status;

    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    public T? Result { get; } = result;
}

/// <summary>
/// Represents a possible service response status.
/// </summary>
public enum ServiceResponseStatus
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
    Forbidden
}
