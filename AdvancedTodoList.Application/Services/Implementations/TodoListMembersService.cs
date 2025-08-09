using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Core.Specifications.Filters;
using AdvancedTodoList.Core.Specifications.Todo;
using Mapster;

namespace AdvancedTodoList.Application.Services.Implementations;

/// <summary>
/// A service that manages to-do list members.
/// </summary>
public class TodoListMembersService(
    ITodoListDependantEntitiesService<TodoListMember, int> helperService,
    ITodoListMembersRepository membersRepository,
    IRepository<TodoListRole, int> rolesRepository,
    IPermissionsChecker permissionsChecker) :
    ITodoListMembersService
{
    private readonly ITodoListDependantEntitiesService<TodoListMember, int> _helperService = helperService;
    private readonly ITodoListMembersRepository _membersRepository = membersRepository;
    private readonly IRepository<TodoListRole, int> _rolesRepository = rolesRepository;
    private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;

    /// <summary>
    /// Gets a page with members of a to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="paginationParameters">Pagination parameters to use.</param>
    /// <param name="filter">Filter parameters to use.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponse<Page<TodoListMemberPreviewDto>>> GetMembersAsync(TodoListContext context,
        PaginationParameters paginationParameters, TodoListMembersFilter filter)
    {
        TodoListMembersSpecification specification = new(context.TodoListId, filter);
        return _helperService.GetPageAsync<TodoListMemberPreviewDto>(
            context, specification, paginationParameters
            );
    }

    /// <summary>
    /// Adds a member to a to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="dto">DTO that contains information needed for adding a member. Supossed to be valid.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public async Task<AddTodoListMemberServiceResult> AddMemberAsync(TodoListContext context, TodoListMemberAddDto dto)
    {
        // Check if user has the permission 
        if (!await _permissionsChecker.HasPermissionAsync(context, x => x.AddMembers))
            return new(TodoListMemberServiceResultStatus.Forbidden);

        // Try to find already existing member
        var member = await _membersRepository.FindAsync(context.TodoListId, dto.UserId);
        // Return error if it exists
        if (member != null) return new(TodoListMemberServiceResultStatus.UserAlreadyAdded);

        // Add member
        var response = await _helperService
            .CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(context, dto);

        return response.Status switch
        {
            ServiceResponseStatus.Success => new(TodoListMemberServiceResultStatus.Success, response.Result),
            ServiceResponseStatus.NotFound => new(TodoListMemberServiceResultStatus.NotFound),
            _ => throw new InvalidOperationException("Invalid to-do lists dependant entities (members) service response.")
        };
    }

    /// <summary>
    /// Updates a role of the member of a to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="memberId">ID of the member.</param>
    /// <param name="dto">DTO that contains information needed for updating a role.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public async Task<TodoListMemberServiceResultStatus> UpdateMemberRoleAsync(TodoListContext context, int memberId, TodoListMemberUpdateRoleDto dto)
    {
        // I think that this method does too much and has many test cases, probably needs refactoring

        // Check if the caller has the permission to assign roles
        MemberPermissionsSpecification specification = new(context.TodoListId, context.CallerId);
        var caller = await _membersRepository.GetAggregateAsync<PermissionsAggregate>(specification);
        if (caller == null || caller.Role == null || !caller.Role.Permissions.AssignRoles)
            return TodoListMemberServiceResultStatus.Forbidden;

        // Check if user has the permission to assign the role
        if (dto.RoleId != null)
        {
            var role = await _rolesRepository.GetByIdAsync(dto.RoleId.Value);
            // Validate RoleId
            if (role == null || role.TodoListId != context.TodoListId)
                return TodoListMemberServiceResultStatus.InvalidRoleId;
            // Check priority
            if (role.Priority <= caller.Role.Priority)
                return TodoListMemberServiceResultStatus.Forbidden;
        }

        // Get the model of a member
        var member = await _membersRepository.GetByIdAsync(memberId);
        // Check if it's valid
        if (member == null || member.TodoListId != context.TodoListId)
            return TodoListMemberServiceResultStatus.NotFound;

        // Check if user has a permission to assign roles to the member
        if (member.RoleId != null)
        {
            // Get the member's role
            var role = await _rolesRepository.GetByIdAsync(member.RoleId.Value) ??
                throw new InvalidOperationException("Member's role is not found");

            if (role.Priority <= caller.Role.Priority)
                return TodoListMemberServiceResultStatus.Forbidden;
        }

        // Update the model
        dto.Adapt(member);
        // Save changes
        await _membersRepository.UpdateAsync(member);

        return TodoListMemberServiceResultStatus.Success;
    }

    /// <summary>
    /// Removes a member from a to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="memberId">ID of the member.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<ServiceResponseStatus> RemoveMemberAsync(TodoListContext context, int memberId)
    {
        return _helperService.DeleteAsync(context, memberId, x => x.RemoveMembers);
    }
}
