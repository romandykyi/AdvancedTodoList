using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using Mapster;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListDependantEntitiesServiceTests : BusinessLogicFixture
{

	private ITodoListDependantEntitiesService<TestTodoListDependantEntity, int> _service;

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<
			ITodoListDependantEntitiesService<TestTodoListDependantEntity, int>>()!;
	}

	[Test]
	public async Task GetPageAsync_ListExists_AppliesSpecification()
	{
		// Arrange
		string todoListId = "ID";
		PaginationParameters parameters = new(2, 5);
		ISpecification<TestTodoListDependantEntity> specification =
			Substitute.For<ISpecification<TestTodoListDependantEntity>>();
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetPageAsync<TestTodoListDependantViewDto>(Arg.Any<PaginationParameters>(),
			Arg.Any<ISpecification<TestTodoListDependantEntity>>())
			.Returns(new Page<TestTodoListDependantViewDto>([], parameters.Page, parameters.PageSize, 0));

		// Act
		var result = await _service.GetPageAsync<TestTodoListDependantViewDto>(todoListId, specification, parameters);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.GetPageAsync<TestTodoListDependantViewDto>(parameters, Arg.Any<ISpecification<TestTodoListDependantEntity>>());
	}

	[Test]
	public async Task GetPageAsync_ListDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		PaginationParameters parameters = new(2, 5);
		ISpecification<TestTodoListDependantEntity> specification =
			Substitute.For<ISpecification<TestTodoListDependantEntity>>();
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		var result = await _service.GetPageAsync<TestTodoListDependantViewDto>(todoListId,
			specification, parameters);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_ReturnsCorrectEntity()
	{
		// Arrange
		string todoListId = "ID";
		TestTodoListDependantEntity entity = TestModels.CreateTestTodoListDependantEntity(todoListId);
		entity.Id = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(todoListId, entity.Id);

		// Assert that returned DTO matches
		var expectedResult = entity.Adapt<TestTodoListDependantViewDto>();
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task GetByIdAsync_TodoListDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		int entityId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(todoListId, entityId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Arrange
		string todoListId = "ID";
		int entityId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entityId)
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(todoListId, entityId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_TodoListExists_AddsEntityToDb()
	{
		// Arrange: initialize a DTO
		string todoListId = "ID";
		TestTodoListDependantCreateDto dto = new("Test");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.AddAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act: call the method
		var result = await _service.CreateAsync<TestTodoListDependantCreateDto,
			TestTodoListDependantViewDto>(todoListId, dto);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.AddAsync(Arg.Is<TestTodoListDependantEntity>(x =>
			x.TestProperty == dto.TestProperty && x.TodoListId == todoListId));
	}

	[Test]
	public async Task CreateAsync_TodoListDoesNotExist_ReturnsNull()
	{
		// Arrange: initialize a DTO
		string todoListId = "ID";
		TestTodoListDependantCreateDto dto = new("Test");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act: call the method
		var result = await _service.CreateAsync<TestTodoListDependantCreateDto,
			TestTodoListDependantViewDto>(todoListId, dto);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task UpdateAsync_EntityExists_Succeeds()
	{
		// Arrange
		string todoListId = "ID";
		var entity = TestModels.CreateTestTodoListDependantEntity(todoListId);
		TestTodoListDependantCreateDto updateDto = new("New test");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.UpdateAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.UpdateAsync(todoListId, entity.Id, updateDto);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that update was called
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.UpdateAsync(Arg.Is<TestTodoListDependantEntity>(x => 
			x.Id == entity.Id && x.TestProperty == updateDto.TestProperty));
	}

	[Test]
	public async Task UpdateAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int entityId = 500;
		TestTodoListDependantCreateDto updateDto = new("New test");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entityId)
			.ReturnsNull();

		// Act
		bool result = await _service.UpdateAsync(todoListId, entityId, updateDto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task UpdateAsync_TodoListDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int entityId = 500;
		TestTodoListDependantCreateDto updateDto = new("New test");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		bool result = await _service.UpdateAsync(todoListId, entityId, updateDto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_EntityExists_Succeeds()
	{
		// Arrange
		string todoListId = "ID";
		var entity = TestModels.CreateTestTodoListDependantEntity(todoListId);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.DeleteAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.DeleteAsync(todoListId, entity.Id);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that delete was called
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.DeleteAsync(Arg.Is<TestTodoListDependantEntity>(x => x.Id == entity.Id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int entityId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entityId)
			.ReturnsNull();
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.DeleteAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.DeleteAsync(todoListId, entityId);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_TodoListDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string todoListId = "ID";
		int entityId = 500;
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(todoListId)
			.Returns(false);

		// Act
		bool result = await _service.DeleteAsync(todoListId, entityId);

		// Assert that result is false
		Assert.That(result, Is.False);
	}
}
