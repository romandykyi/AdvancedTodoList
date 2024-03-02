using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using Mapster;

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
	public async Task GetByIdAsync_EntityExists_ReturnsCorrectEntity()
	{
		// Arrange
		TodoList todoList = TestModels.CreateTestTodoList();
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);

		// Act
		var result = await _service.GetByIdAsync(todoList.Id);

		// Assert that returned DTO matches
		var expectedResult = todoList.Adapt<TodoListGetByIdDto>();
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoListId)
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync(todoListId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_AddsEntityToDb()
	{
		// Arrange: initialize a DTO
		string callerId = "CallerId";
		TodoListCreateDto dto = new("Test entity", "...");
		WebApplicationFactory.TodoListsRepository
			.AddAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act: call the method
		var result = await _service.CreateAsync(dto, callerId);

		// Assert that method was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.AddAsync(Arg.Is<TodoList>(x => x.Name == dto.Name && 
			x.Description == dto.Description && x.OwnerId == callerId));
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
