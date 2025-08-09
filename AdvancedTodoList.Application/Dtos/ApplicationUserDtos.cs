using AdvancedTodoList.Core.Models.Auth;

namespace AdvancedTodoList.Application.Dtos;

/// <summary>
/// Represents a minimal view DTO for <see cref="ApplicationUser" />
/// </summary>
public record ApplicationUserPreviewDto(string Id, string UserName);

/// <summary>
/// Represents a view DTO for <see cref="ApplicationUser" />
/// </summary>
public record ApplicationUserViewDto(string Id, string Email, string UserName, string FirstName, string LastName);
