namespace AdvancedTodoList.Application.Options;

/// <summary>
/// A class that contains invitation link options.
/// </summary>
public class InvitationLinkOptions
{
    /// <summary>
    /// Size of the refresh token in bytes.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Days before token expires.
    /// </summary>
    public int ExpirationDays { get; set; }
}
