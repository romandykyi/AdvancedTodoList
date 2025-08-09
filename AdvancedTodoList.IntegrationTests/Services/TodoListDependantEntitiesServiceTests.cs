using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using Mapster;
using NUnit.Framework.Internal;
using AdvancedTodoList.Application.Services.Definitions;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListDependantEntitiesServiceTests : BusinessLogicFixture
{
	private readonly TodoListContext TestContext = new("ListId", "TestUserId");
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
		PaginationParameters parameters = new(2, 5);
		ISpecification<TestTodoListDependantEntity> specification =
			Substitute.For<ISpecification<TestTodoListDependantEntity>>();
		Page<TestTodoListDependantViewDto> page = new([], parameters.Page, parameters.PageSize, 0);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(TestContext.TodoListId)
			.Returns(true);
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetPageAsync<TestTodoListDependantViewDto>(Arg.Any<PaginationParameters>(),
			Arg.Any<ISpecification<TestTodoListDependantEntity>>())
			.Returns(page);

		// Act
		var result = await _service.GetPageAsync<TestTodoListDependantViewDto>(TestContext, specification, parameters);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Success));
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.GetPageAsync<TestTodoListDependantViewDto>(parameters, Arg.Any<ISpecification<TestTodoListDependantEntity>>());
	}

	[Test]
	public async Task GetPageAsync_ListDoesNotExist_ReturnsNotFoundStatus()
	{
		// Arrange
		PaginationParameters parameters = new(2, 5);
		ISpecification<TestTodoListDependantEntity> specification =
			Substitute.For<ISpecification<TestTodoListDependantEntity>>();
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(TestContext.TodoListId)
			.Returns(false);
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);

		// Act
		var result = await _service.GetPageAsync<TestTodoListDependantViewDto>(TestContext,
			specification, parameters);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task GetPageAsync_UserIsNotMember_ReturnsForbidden()
	{
		// Arrange
		PaginationParameters parameters = new(2, 5);
		ISpecification<TestTodoListDependantEntity> specification =
			Substitute.For<ISpecification<TestTodoListDependantEntity>>();
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(TestContext.TodoListId)
			.Returns(true);
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(false);

		// Act
		var result = await _service.GetPageAsync<TestTodoListDependantViewDto>(TestContext,
			specification, parameters);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_SucceedsAndReturnsCorrectEntity()
	{
		// Arrange
		TestTodoListDependantEntity entity = TestModels.CreateTestTodoListDependantEntity(
			TestContext.TodoListId);
		entity.Id = 500;
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(TestContext, entity.Id);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that returned DTO matches
		var expectedResult = entity.Adapt<TestTodoListDependantViewDto>();
		Assert.That(result.Result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task GetByIdAsync_WrongTodoList_ReturnsNotFound()
	{
		// Arrange
		TestTodoListDependantEntity entity = TestModels.CreateTestTodoListDependantEntity("WrongTodoListId");
		entity.Id = 500;
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(TestContext, entity.Id);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		int entityId = 500;
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entityId)
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(TestContext, entityId);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task GetByIdAsync_UserIsNotMember_ReturnsForbidden()
	{
		// Arrange
		TestTodoListDependantEntity entity = TestModels.CreateTestTodoListDependantEntity(
			TestContext.TodoListId);
		int entityId = 500;
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(TestContext)
			.Returns(false);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);

		// Act
		var result = await _service.GetByIdAsync<TestTodoListDependantViewDto>(TestContext, entityId);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task CreateAsync_ValidCall_AddsEntityToDb()
	{
		// Arrange
		TestTodoListDependantCreateDto dto = new("Test");
		RolePermissions validPermissions = new(AddItems: true);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[1])(validPermissions));
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(TestContext.TodoListId)
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.AddAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.CreateAsync<TestTodoListDependantCreateDto,
			TestTodoListDependantViewDto>(TestContext, dto, x => x.AddItems);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Success));
			Assert.That(result.Result, Is.Not.Null);
		});
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.AddAsync(Arg.Is<TestTodoListDependantEntity>(x =>
			x.TestProperty == dto.TestProperty && x.TodoListId == TestContext.TodoListId &&
			x.OwnerId == TestContext.CallerId));
	}

	[Test]
	public async Task CreateAsync_TodoListDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		TestTodoListDependantCreateDto dto = new("Test");
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(TestContext.TodoListId)
			.Returns(false);

		// Act: call the method
		var result = await _service.CreateAsync<TestTodoListDependantCreateDto,
			TestTodoListDependantViewDto>(TestContext, dto, x => x.AddItems);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task CreateAsync_UserHasNoPermssion_ReturnsForbidden()
	{
		// Arrange
		TestTodoListDependantCreateDto dto = new("Test");
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(TestContext.TodoListId)
			.Returns(true);

		// Act: call the method
		var result = await _service.CreateAsync<TestTodoListDependantCreateDto,
			TestTodoListDependantViewDto>(TestContext, dto, x => x.AddItems);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task UpdateAsync_EntityExists_Succeeds()
	{
		// Arrange
		var entity = TestModels.CreateTestTodoListDependantEntity(TestContext.TodoListId);
		TestTodoListDependantCreateDto updateDto = new("New test");
		RolePermissions validPermissions = new(EditItems: true);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TestTodoListDependantEntity, int>(TestContext,
			Arg.Any<TestTodoListDependantEntity>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[2])(validPermissions));
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.UpdateAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.UpdateAsync(TestContext, entity.Id, updateDto, x => x.EditItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that update was called
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.UpdateAsync(Arg.Is<TestTodoListDependantEntity>(x =>
			x.Id == entity.Id && x.TestProperty == updateDto.TestProperty));
	}

	[Test]
	public async Task UpdateAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		int entityId = 500;
		TestTodoListDependantCreateDto updateDto = new("New test");
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entityId)
			.ReturnsNull();

		// Act
		var result = await _service.UpdateAsync(TestContext, entityId, updateDto, x => x.EditItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task UpdateAsync_WrongTodoListId_ReturnsNotFound()
	{
		// Arrange
		var entity = TestModels.CreateTestTodoListDependantEntity("Wrong to-do list ID");
		int entityId = 500;
		TestTodoListDependantCreateDto updateDto = new("New test");
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TestTodoListDependantEntity, int>(TestContext,
			Arg.Any<TestTodoListDependantEntity>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);

		// Act
		var result = await _service.UpdateAsync(TestContext, entityId, updateDto, x => x.EditItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task UpdateAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		TestTodoListDependantCreateDto updateDto = new("New test");
		var entity = TestModels.CreateTestTodoListDependantEntity(TestContext.TodoListId);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TestTodoListDependantEntity, int>(TestContext,
			Arg.Any<TestTodoListDependantEntity>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);

		// Act
		var result = await _service.UpdateAsync(TestContext, entity.Id, updateDto, x => x.EditItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task DeleteAsync_ValidCall_Succeeds()
	{
		// Arrange
		var entity = TestModels.CreateTestTodoListDependantEntity(TestContext.TodoListId);
		RolePermissions validPermissions = new(DeleteItems: true);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TestTodoListDependantEntity, int>(TestContext,
			Arg.Any<TestTodoListDependantEntity>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[2])(validPermissions));
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.DeleteAsync(Arg.Any<TestTodoListDependantEntity>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.DeleteAsync(TestContext, entity.Id, x => x.DeleteItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that delete was called
		await WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.Received()
			.DeleteAsync(Arg.Is<TestTodoListDependantEntity>(x => x.Id == entity.Id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		int entityId = 500;
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entityId)
			.ReturnsNull();

		// Act
		var result = await _service.DeleteAsync(TestContext, entityId, x => x.DeleteItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task DeleteAsync_InvalidTodoListId_ReturnsNotFound()
	{
		// Arrange
		int entityId = 500;
		var entity = TestModels.CreateTestTodoListDependantEntity("Wrong to-do list ID");
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TestTodoListDependantEntity, int>(TestContext,
			Arg.Any<TestTodoListDependantEntity>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);

		// Act
		var result = await _service.DeleteAsync(TestContext, entityId, x => x.DeleteItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task DeleteAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		var entity = TestModels.CreateTestTodoListDependantEntity(TestContext.TodoListId);
		WebApplicationFactory.TestTodoListDependantEntitiesRepository
			.GetByIdAsync(entity.Id)
			.Returns(entity);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TestTodoListDependantEntity, int>(TestContext,
			Arg.Any<TestTodoListDependantEntity>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);

		// Act
		var result = await _service.DeleteAsync(TestContext, entity.Id, x => x.DeleteItems);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}
}
