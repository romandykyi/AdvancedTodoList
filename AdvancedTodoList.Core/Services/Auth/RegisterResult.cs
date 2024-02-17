namespace AdvancedTodoList.Core.Services.Auth;

/// <summary>
/// Class that represents a result of register operation.
/// </summary>
public class RegisterResult
{
	/// <summary>
	/// Flag which determines whether result reports succeess.
	/// </summary>
	public bool IsSuccess { get; private init; }
	/// <summary>
	/// Error messages.
	/// </summary>
	public IEnumerable<RegisterError> Errors { get; private init; }

	private RegisterResult(bool isSuccess, IEnumerable<RegisterError> errors)
	{
		IsSuccess = isSuccess;
		Errors = errors;
	}

	/// <summary>
	/// Returns a result that reports success.
	/// </summary>
	/// <returns>
	/// A result that reports success.
	/// </returns>
	public static RegisterResult Success() => new(true, Array.Empty<RegisterError>());

	/// <summary>
	/// Returns a result that reports failure.
	/// </summary>
	/// <param name="errors">Error messages.</param>
	/// <returns>
	/// A result that reports failure.
	/// </returns>
	public static RegisterResult Failure(IEnumerable<RegisterError> errors) => new(false, errors);
}

/// <summary>
/// Record that represents a register error.
/// </summary>
/// <param name="Property">Property that caused an error or '$' if error was not caused by a property.</param>
/// <param name="Message">An error message.</param>
public record RegisterError(string Property, string Message);
