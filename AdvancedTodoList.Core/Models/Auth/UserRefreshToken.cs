using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTodoList.Core.Models.Auth;

/// <summary>
/// Model which represents a user's refresh token.
/// </summary>
public class UserRefreshToken : IEntity<int>
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// A foreign key to the user who owns the token.
    /// </summary>
    [ForeignKey(nameof(User))]
    public required string UserId { get; set; }

    /// <summary>
    /// A value of the token.
    /// </summary>
    public required string Token { get; set; }
    /// <summary>
    /// Date after which token becomes invalid.
    /// </summary>
    public required DateTime ValidTo { get; set; }

    /// <summary>
    /// A navigation property for a user which owns the token.
    /// </summary>
    public ApplicationUser User { get; set; } = null!;
}
