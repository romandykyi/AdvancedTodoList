namespace AdvancedTodoList.Application.Validation;

/// <summary>
/// Class that contains validation error codes.
/// </summary>
public static class ValidationErrorCodes
{
    /// <summary>
    /// Required property is null or empty.
    /// </summary>
    public const string PropertyRequired = "100";
    /// <summary>
    /// Length of the property exceeds maximum possible value.
    /// </summary>
    public const string PropertyTooLong = "200";
    /// <summary>
    /// Value of the property is out of range.
    /// </summary>
    public const string PropertyOutOfRange = "300";
    /// <summary>
    /// Property is invalid foreign key.
    /// </summary>
    public const string InvalidForeignKey = "400";
    /// <summary>
    /// Property is invalid email.
    /// </summary>
    public const string InvalidEmail = "500";
}
