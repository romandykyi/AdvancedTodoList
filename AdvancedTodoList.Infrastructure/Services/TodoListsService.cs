using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Specifications;
using Mapster;

namespace AdvancedTodoList.Infrastructure.Services;

/// <summary>
/// A service that manages to-do lists.
/// </summary>
public class TodoListsService(
	IRepository<TodoList, string> todoListsRepository,
	IRepository<TodoListRole, int> rolesRepository,
	ITodoListMembersRepository membersRepository,
	IUnitOfWork unitOfWork
	) : ITodoListsService
{
	private readonly IRepository<TodoList, string> _todoListsRepository = todoListsRepository;
	private readonly IRepository<TodoListRole, int> _rolesRepository = rolesRepository;
	private readonly IRepository<TodoListMember, int> _membersRepository = membersRepository;
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	/// <summary>
	/// Retrieves a to-do list by its ID asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to retrieve.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// a <see cref="TodoListGetByIdDto"/> object if the specified ID is found;
	/// otherwise, returns <see langword="null"/>.
	/// </returns>
	public Task<TodoListGetByIdDto?> GetByIdAsync(string id)
	{
		TodoListAggregateSpecification specification = new(id);
		return _todoListsRepository.GetAggregateAsync<TodoListGetByIdDto>(specification);
	}

	/// <summary>
	/// Creates a new to-do list asynchronously.
	/// </summary>
	/// <remarks>
	/// This method also creates an "Owner" role with all permissions and assigns the caller to it.
	/// </remarks>
	/// <param name="dto">The DTO containing information for creating the to-do list.</param>
	/// <param name="callerId">ID of the user who creates the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains the created <see cref="TodoList"/> mapped to 
	/// <see cref="TodoListGetByIdDto"/>.
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

		// Return DTO of created model
		return todoList.Adapt<TodoListGetByIdDto>();
	}

	/// <summary>
	/// Edits a to-do list asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <param name="dto">The DTO containing information for editing the to-do list.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if entity was not found.
	/// </returns>
	public async Task<bool> EditAsync(string id, TodoListCreateDto dto)
	{
		// Get the model
		var todoList = await _todoListsRepository.GetByIdAsync(id);
		// Return false if the model doesn't exist
		if (todoList == null) return false;

		// Update the model
		dto.Adapt(todoList);
		await _todoListsRepository.UpdateAsync(todoList);

		return true;
	}

	/// <summary>
	/// Deletes a to-do list asynchronously.
	/// </summary>
	/// <param name="id">The ID of the to-do list to edit.</param>
	/// <returns>
	/// A task representing the asynchronous operation. 
	/// The task result contains <see langword="true"/> on success;
	/// otherwise <see langword="false"/> if entity was not found.
	/// </returns>
	public async Task<bool> DeleteAsync(string id)
	{
		// Get the model
		var todoList = await _todoListsRepository.GetByIdAsync(id);
		// Return false if the model doesn't exist
		if (todoList == null) return false;

		// Delete the model
		await _todoListsRepository.DeleteAsync(todoList);

		return true;
	}
}
