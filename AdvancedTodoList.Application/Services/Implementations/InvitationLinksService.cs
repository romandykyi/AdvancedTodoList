using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Options;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Specifications;
using Mapster;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace AdvancedTodoList.Application.Services.Implementations;

/// <summary>
/// A service that manages invitation links.
/// </summary>
public class InvitationLinksService(
    IPermissionsChecker permissionsChecker,
    IInvitationLinksRepository linksRepository,
    ITodoListMembersRepository membersRepository,
    IEntityExistenceChecker existenceChecker,
    IOptions<InvitationLinkOptions> options
    ) : IInvitationLinksService
{
    private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;
    private readonly IInvitationLinksRepository _linksRepository = linksRepository;
    private readonly ITodoListMembersRepository _membersRepository = membersRepository;
    private readonly IEntityExistenceChecker _existenceChecker = existenceChecker;
    private readonly InvitationLinkOptions _options = options.Value;

    /// <summary>
    /// Joins the caller to the to-do list by invitation list asynchronously.
    /// </summary>
    /// <param name="callerId">ID of the caller.</param>
    /// <param name="invitationLinkValue">Invitation link to use.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<JoinByInvitationLinkResult> JoinAsync(string callerId, string invitationLinkValue)
    {
        // Try to find a link
        InvitationLink? invitationLink = await _linksRepository.FindAsync(invitationLinkValue);
        if (invitationLink == null) return new(JoinByInvitationLinkStatus.NotFound);

        // Check if link is still valid
        if (invitationLink.ValidTo < DateTime.UtcNow) return new(JoinByInvitationLinkStatus.Expired);

        // Check if user is not already a member
        var member = await _membersRepository.FindAsync(invitationLink.TodoListId, callerId);
        if (member != null) return new(JoinByInvitationLinkStatus.UserIsAlreadyMember);

        // Add a new member
        TodoListMember newMember = new()
        {
            TodoListId = invitationLink.TodoListId,
            UserId = callerId
        };
        await _membersRepository.AddAsync(newMember);
        var dto = newMember.Adapt<TodoListMemberMinimalViewDto>();
        return new(JoinByInvitationLinkStatus.Success, dto);
    }

    /// <summary>
    /// Gets invitation links associated with the to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context of the operation.</param>
    /// <param name="parameters">Pagination parameters to use.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<ServiceResponse<Page<InvitationLinkDto>>> GetInvitationLinksAsync(TodoListContext context,
        PaginationParameters parameters)
    {
        // Check if to-do list exists
        if (!await _existenceChecker.ExistsAsync<TodoList, string>(context.TodoListId))
            return new(ServiceResponseStatus.NotFound);

        // Check if user has the permission to see links
        if (!await _permissionsChecker.HasPermissionAsync(
            context, x => x.ManageInvitationLinks || x.AddMembers))
            return new(ServiceResponseStatus.Forbidden);

        // Get the requested page
        InvitationLinksSpecification specification = new(context.TodoListId);
        var page = await _linksRepository
            .GetPageAsync<InvitationLinkDto>(parameters, specification);
        // Return the page
        return new(ServiceResponseStatus.Success, page);
    }

    /// <summary>
    /// Creates an invitation link associated to the to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<ServiceResponse<InvitationLinkDto>> CreateAsync(TodoListContext context)
    {
        // Check if to-do list exists
        if (!await _existenceChecker.ExistsAsync<TodoList, string>(context.TodoListId))
            return new(ServiceResponseStatus.NotFound);
        // Check if the user has the permission
        if (!await _permissionsChecker.HasPermissionAsync(context, x => x.AddMembers))
            return new(ServiceResponseStatus.Forbidden);

        // Generate the link
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] valueBytes = new byte[_options.Size];
        rng.GetBytes(valueBytes);

        // Create the link
        InvitationLink link = new()
        {
            TodoListId = context.TodoListId,
            Value = Convert.ToBase64String(valueBytes),
            ValidTo = DateTime.UtcNow.AddDays(_options.ExpirationDays)
        };
        // Save it
        await _linksRepository.AddAsync(link);
        // Map it to DTO and return
        var result = link.Adapt<InvitationLinkDto>();
        return new(ServiceResponseStatus.Success, result);
    }

    /// <summary>
    /// Deletes an invitation link associted to the to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="linkId">ID of the link.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<ServiceResponseStatus> DeleteAsync(TodoListContext context, int linkId)
    {
        // Get the model of a link
        var link = await _linksRepository.GetByIdAsync(linkId);
        // Check if it's valid
        if (link == null || link.TodoListId != context.TodoListId)
            return ServiceResponseStatus.NotFound;
        // Check if user has the permission
        if (!await _permissionsChecker.HasPermissionAsync(context, x => x.ManageInvitationLinks))
            return ServiceResponseStatus.Forbidden;

        // Delete the link
        await _linksRepository.DeleteAsync(link);

        return ServiceResponseStatus.Success;
    }
}
