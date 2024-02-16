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
	public IEnumerable<string> Errors { get; private init; }

	private RegisterResult(bool isSuccess, IEnumerable<string> errors) 
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
	public static RegisterResult Success() => new(true, Array.Empty<string>());

	/// <summary>
	/// Returns a result that reports failure.
	/// </summary>
	/// <param name="errors">Error messages.</param>
	/// <returns>
	/// A result that reports failure.
	/// </returns>
	public static RegisterResult Failure(IEnumerable<string> errors) => new(false, errors);
}
