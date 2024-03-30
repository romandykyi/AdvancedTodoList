namespace AdvancedTodoList.Core.Dtos;

/// <summary>
/// Represents a DTO for invitation link view.
/// </summary>
public record InvitationLinkDto(int Id, string Value, DateTime ValidTo);
