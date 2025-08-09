using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Pagination;

namespace AdvancedTodoList.Application.Services.Definitions;

/// <summary>
/// An interface for a service that manages invitation links.
/// </summary>
public interface IInvitationLinksService
{
    /// <summary>
    /// Joins the caller to the to-do list by invitation list asynchronously.
    /// </summary>
    /// <param name="callerId">ID of the caller.</param>
    /// <param name="invitationLinkValue">Invitation link to use.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    Task<JoinByInvitationLinkResult> JoinAsync(string callerId, string invitationLinkValue);

    /// <summary>
    /// Gets invitation links associated with the to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context of the operation.</param>
    /// <param name="parameters">Pagination parameters to use.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    Task<ServiceResponse<Page<InvitationLinkDto>>> GetInvitationLinksAsync(TodoListContext context,
        PaginationParameters parameters);

    /// <summary>
    /// Creates an invitation link associated to the to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    Task<ServiceResponse<InvitationLinkDto>> CreateAsync(TodoListContext context);

    /// <summary>
    /// Deletes an invitation link associted to the to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="linkId">ID of the link.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int linkId);
}
