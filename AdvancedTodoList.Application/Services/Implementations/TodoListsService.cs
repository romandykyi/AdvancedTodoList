using AdvancedTodoList.Application.Services.Definitions;
using AdvancedTodoList.Application.Services.Definitions.Auth;
using AdvancedTodoList.Application.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Specifications.Filters;
using AdvancedTodoList.Core.Specifications.Todo;
using Mapster;

namespace AdvancedTodoList.Application.Services.Implementations;

/// <summary>
/// A service that manages to-do lists.
/// </summary>
public class TodoListsService(
    IPermissionsChecker permissionsChecker,
    IRepository<TodoList, string> todoListsRepository,
    IRepository<TodoListRole, int> rolesRepository,
    ITodoListMembersRepository membersRepository,
    IUnitOfWork unitOfWork
    ) : ITodoListsService
{
    private readonly IPermissionsChecker _permissionsChecker = permissionsChecker;
    private readonly IRepository<TodoList, string> _todoListsRepository = todoListsRepository;
    private readonly IRepository<TodoListRole, int> _rolesRepository = rolesRepository;
    private readonly IRepository<TodoListMember, int> _membersRepository = membersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Retrieves a page of to-do lists, with the requirement that the user 
    /// is a member of those lists.
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="paginationParameters">Pagination parameters to use.</param>
    /// <param name="filter">Filter parameters to apply.</param>
    /// <returns>
    /// A task representing the asynchronous operation containing the result of operation.
    /// </returns>
    public Task<Page<TodoListPreviewDto>> GetListsOfUserAsync(string userId,
        PaginationParameters paginationParameters, TodoListsFilter filter)
    {
        TodoListsSpecification specification = new(userId, filter);
        return _todoListsRepository.GetPageAsync<TodoListPreviewDto>(
            paginationParameters, specification);
    }

    /// <summary>
    /// Retrieves a to-do list by its ID asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<ServiceResponse<TodoListGetByIdDto>> GetByIdAsync(TodoListContext context)
    {
        TodoListAggregateSpecification specification = new(context.TodoListId);
        var result = await _todoListsRepository.GetAggregateAsync<TodoListGetByIdDto>(specification);
        // Check if to-do list exists
        if (result == null) return new(ServiceResponseStatus.NotFound);

        // Check if the user has a permission to view the to-do list
        if (!await _permissionsChecker.IsMemberOfListAsync(context))
            return new(ServiceResponseStatus.Forbidden);

        return new(ServiceResponseStatus.Success, result);
    }

    /// <summary>
    /// Creates a new to-do list asynchronously.
    /// </summary>
    /// <remarks>
    /// This method should also create an "Owner" role with all permissions and assign the caller to it.
    /// </remarks>
    /// <param name="dto">The DTO containing information for creating the to-do list.</param>
    /// <param name="callerId">ID of the user who creates the to-do list.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a created model mapped to <see cref="TodoListGetByIdDto" />.
    /// </returns>
    public async Task<TodoListGetByIdDto> CreateAsync(TodoListCreateDto dto, string callerId)
    {
        // Map DTO to the model
        var todoList = dto.Adapt<TodoList>();
        // Set the owner
        todoList.OwnerId = callerId;

        // Begin a transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Add the list to the database
            await _todoListsRepository.AddAsync(todoList);

            // Create an "Owner" role
            TodoListRole ownerRole = new()
            {
                Name = "Owner",
                Priority = 0,
                TodoListId = todoList.Id,
                Permissions = RolePermissions.All
            };
            await _rolesRepository.AddAsync(ownerRole);

            // Assign the caller to it
            TodoListMember member = new()
            {
                UserId = callerId,
                TodoListId = todoList.Id,
                RoleId = ownerRole.Id
            };
            await _membersRepository.AddAsync(member);
        }
        catch (Exception)
        {
            // Rollback in a case of error
            await _unitOfWork.RollbackAsync();
            throw;
        }

        // Commit changes
        await _unitOfWork.CommitAsync();

        // Return a DTO of created model
        return todoList.Adapt<TodoListGetByIdDto>();
    }

    /// <summary>
    /// Edits a to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <param name="dto">The DTO containing information for editing the to-do list.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<ServiceResponseStatus> EditAsync(TodoListContext context, TodoListCreateDto dto)
    {
        // Get the model
        var todoList = await _todoListsRepository.GetByIdAsync(context.TodoListId);
        // Return NotFound if the model doesn't exist
        if (todoList == null) return ServiceResponseStatus.NotFound;
        // Check if the user has a permission to edit the list
        if (!await _permissionsChecker.CanTouchEntityAsync<TodoList, string>(
            context, todoList, x => x.EditItems))
        {
            return ServiceResponseStatus.Forbidden;
        }

        // Update the model
        dto.Adapt(todoList);
        await _todoListsRepository.UpdateAsync(todoList);

        return ServiceResponseStatus.Success;
    }

    /// <summary>
    /// Deletes a to-do list asynchronously.
    /// </summary>
    /// <param name="context">To-do list context.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task contains
    /// a result of the operation.
    /// </returns>
    public async Task<ServiceResponseStatus> DeleteAsync(TodoListContext context)
    {
        // Get the model
        var todoList = await _todoListsRepository.GetByIdAsync(context.TodoListId);
        // Return NotFound if the model doesn't exist
        if (todoList == null) return ServiceResponseStatus.NotFound;
        // Check if the user is an owner of the list
        if (todoList.OwnerId != context.CallerId) return ServiceResponseStatus.Forbidden;

        // Delete the model
        await _todoListsRepository.DeleteAsync(todoList);

        return ServiceResponseStatus.Success;
    }
}
