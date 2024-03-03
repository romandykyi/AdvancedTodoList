using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using NSubstitute.ExceptionExtensions;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListsServiceTests : BusinessLogicFixture
{
	private ITodoListsService _service;

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoListsService>()!;
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_AppliesTodoListAggregateSpecification()
	{
		// Arrange
		string todoListId = "TodoListId";
		TodoListGetByIdDto dto = new("Id", "name", "", new("Id", "User"));
		WebApplicationFactory.TodoListsRepository
			.GetAggregateAsync<TodoListGetByIdDto>(Arg.Any<ISpecification<TodoList>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync(todoListId);

		// Assert that valid specification was applied
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.GetAggregateAsync<TodoListGetByIdDto>(
			Arg.Is<TodoListAggregateSpecification>(x => x.Id == todoListId));
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		WebApplicationFactory.TodoListsRepository
			.GetAggregateAsync<TodoListGetByIdDto>(Arg.Any<ISpecification<TodoList>>())
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync(todoListId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_AddsEntityToDb()
	{
		// Arrange
		string callerId = "CallerId";
		string listId = "ListId";
		int ownerRoleId = 777;
		TodoListCreateDto dto = new("Test entity", "...");
		WebApplicationFactory.TodoListsRepository
			.AddAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult)
			.AndDoes(x => ((TodoList)x[0]).Id = listId);
		WebApplicationFactory.TodoListRolesRepository
			.AddAsync(Arg.Any<TodoListRole>())
			.Returns(Task.FromResult)
			.AndDoes(x => ((TodoListRole)x[0]).Id = ownerRoleId);
		WebApplicationFactory.TodoListMembersRepository
			.AddAsync(Arg.Any<TodoListMember>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.CreateAsync(dto, callerId);

		// Assert that list was created
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.AddAsync(Arg.Is<TodoList>(x => x.Name == dto.Name &&
			x.Description == dto.Description && x.OwnerId == callerId));
		// Assert that "Owner" role was created
		await WebApplicationFactory.TodoListRolesRepository
			.Received()
			.AddAsync(Arg.Is<TodoListRole>(x => x.Priority == 0 && 
			x.TodoListId == listId && x.Permissions == RolePermissions.All));
		// Assert that the caller was assigned to the "Owner" role
		await WebApplicationFactory.TodoListMembersRepository
			.Received()
			.AddAsync(Arg.Is<TodoListMember>(x => x.TodoListId == listId &&
			x.UserId == callerId && x.RoleId == ownerRoleId));
		// Assert that changes were commited
		await WebApplicationFactory.UnitOfWork
			.Received()
			.CommitAsync();
	}

	[Test]
	public async Task CreateAsync_OnException_RethrowsAndRollbacksChanges()
	{
		// Arrange
		string callerId = "CallerId";
		TodoListCreateDto dto = new("Test entity", "...");
		WebApplicationFactory.TodoListsRepository
			.AddAsync(Arg.Any<TodoList>())
			.Throws<Exception>();

		// Act/Assert
		Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto, callerId));

		// Assert
		await WebApplicationFactory.UnitOfWork
			.Received()
			.RollbackAsync();

	}

	[Test]
	public async Task EditAsync_EntityExists_Succeeds()
	{
		// Arrange
		var todoList = TestModels.CreateTestTodoList();
		TodoListCreateDto updateDto = new("NewName", "NewDescription");
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);
		WebApplicationFactory.TodoListsRepository
			.UpdateAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.EditAsync(todoList.Id, updateDto);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that update was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.UpdateAsync(Arg.Is<TodoList>(x => x.Id == todoList.Id &&
				x.Name == updateDto.Name &&
				x.Description == updateDto.Description));
	}

	[Test]
	public async Task EditAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string id = "ID";
		TodoListCreateDto updateDto = new("Name", "Description");
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(id)
			.ReturnsNull();

		// Act
		bool result = await _service.EditAsync(id, updateDto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_EntityExists_Succeeds()
	{
		// Arrange
		var todoList = TestModels.CreateTestTodoList();
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);
		WebApplicationFactory.TodoListsRepository
			.DeleteAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.DeleteAsync(todoList.Id);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that delete was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.DeleteAsync(Arg.Is<TodoList>(x => x.Id == todoList.Id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string id = "ID";
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(id)
			.ReturnsNull();

		// Act
		bool result = await _service.DeleteAsync(id);

		// Assert that result is false
		Assert.That(result, Is.False);
	}
}
