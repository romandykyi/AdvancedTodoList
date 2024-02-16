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
	/// Error message.
	/// </summary>
	public string? Error { get; private init; }

	private RegisterResult() { }

	/// <summary>
	/// Returns a result that reports success.
	/// </summary>
	/// <returns>
	/// A result that reports success.
	/// </returns>
	public static RegisterResult Success() => new() { IsSuccess = true };

	/// <summary>
	/// Returns a result that reports failure.
	/// </summary>
	/// <param name="error">Error message.</param>
	/// <returns>
	/// A result that reports failure.
	/// </returns>
	public static RegisterResult Failure(string error) => new() { IsSuccess = false, Error = error };
}
