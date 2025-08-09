using System.Security.Claims;

namespace AdvancedTodoList.WebApp.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets a user ID or <see langword="null" /> if it's not found.
    /// </summary>
    /// <returns>
    /// User ID or <see langword="null" /> if it's not found.
    /// </returns>
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var idClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null) return null;
        return idClaim.Value;
    }
}
