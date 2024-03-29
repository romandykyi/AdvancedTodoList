﻿using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
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
	private const string TestUserId = "UserId";

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoListsService>()!;
	}

	[Test]
	public async Task GetListsOfUserAsync_AppliesTodoListsSpecification()
	{
		// Arrange
		PaginationParameters parameters = new(2, 5);
		Page<TodoListPreviewDto> page = new([], 1, 1, 1);
		TodoListsFilter filter = new(Name: "Name");
		const string userId = "abcd";
		WebApplicationFactory.TodoListsRepository
			.GetPageAsync<TodoListPreviewDto>(Arg.Any<PaginationParameters>(), Arg.Any<ISpecification<TodoList>>())
			.Returns(page);

		// Act
		var result = await _service.GetListsOfUserAsync(userId, parameters, filter);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.GetPageAsync<TodoListPreviewDto>(parameters, Arg.Is<TodoListsSpecification>(
				x => x.Filter == filter && x.UserId == userId));
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_AppliesTodoListAggregateSpecification()
	{
		// Arrange
		string todoListId = "TodoListId";
		TodoListGetByIdDto dto = new("Id", "name", "", new("Id", "User"));
		TodoListContext context = new(todoListId, TestUserId);
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(new(todoListId, TestUserId))
			.Returns(true);
		WebApplicationFactory.TodoListsRepository
			.GetAggregateAsync<TodoListGetByIdDto>(Arg.Any<ISpecification<TodoList>>())
			.Returns(dto);

		// Act
		var result = await _service.GetByIdAsync(context);

		// Assert that valid specification was applied
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.GetAggregateAsync<TodoListGetByIdDto>(
			Arg.Is<TodoListAggregateSpecification>(x => x.Id == todoListId));
	}

	[Test]
	public async Task GetByIdAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		string todoListId = "ID";
		TodoListGetByIdDto dto = new("Id", "name", "", new("Id", "User"));
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(new(todoListId, TestUserId))
			.Returns(false);
		WebApplicationFactory.TodoListsRepository
			.GetAggregateAsync<TodoListGetByIdDto>(Arg.Any<ISpecification<TodoList>>())
			.Returns(dto);
		TodoListContext context = new(todoListId, TestUserId);

		// Act
		var result = await _service.GetByIdAsync(context);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		string todoListId = "ID";
		TodoListContext context = new(todoListId, TestUserId);
		WebApplicationFactory.PermissionsChecker
			.IsMemberOfListAsync(new(todoListId, TestUserId))
			.Returns(true);
		WebApplicationFactory.TodoListsRepository
			.GetAggregateAsync<TodoListGetByIdDto>(Arg.Any<ISpecification<TodoList>>())
			.ReturnsNull();

		// Act
		var result = await _service.GetByIdAsync(context);

		// Assert
		Assert.That(result.Status, Is.EqualTo(ServiceResponseStatus.NotFound));
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
		RolePermissions validPermissions = new(EditItems: true);
		TodoListCreateDto updateDto = new("NewName", "NewDescription");
		TodoListContext context = new(todoList.Id, TestUserId);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TodoList, string>(new(todoList.Id, TestUserId),
			Arg.Any<TodoList>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(x => ((Func<RolePermissions, bool>)x[2])(validPermissions));
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);
		WebApplicationFactory.TodoListsRepository
			.UpdateAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.EditAsync(context, updateDto);

		// Assert that result indicates success
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that update was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.UpdateAsync(Arg.Is<TodoList>(x => x.Id == todoList.Id &&
				x.Name == updateDto.Name &&
				x.Description == updateDto.Description));
	}

	[Test]
	public async Task EditAsync_UserHasNoPermission_ReturnsForbidden()
	{
		// Arrange
		var todoList = TestModels.CreateTestTodoList();
		TodoListCreateDto updateDto = new("Name", "Description");
		TodoListContext context = new(todoList.Id, TestUserId);
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);
		WebApplicationFactory.PermissionsChecker
			.CanTouchEntityAsync<TodoList, string>(new(todoList.Id, TestUserId),
			Arg.Any<TodoList>(), Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);

		// Act
		var result = await _service.EditAsync(context, updateDto);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
	}

	[Test]
	public async Task EditAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		string todoListId = "ID";
		TodoListCreateDto updateDto = new("Name", "Description");
		TodoListContext context = new(todoListId, TestUserId);
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoListId)
			.ReturnsNull();

		// Act
		var result = await _service.EditAsync(context, updateDto);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task DeleteAsync_UserIsOwner_Succeeds()
	{
		// Arrange
		var todoList = TestModels.CreateTestTodoList(TestUserId);
		TodoListContext context = new(todoList.Id, TestUserId);
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);
		WebApplicationFactory.TodoListsRepository
			.DeleteAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act
		var result = await _service.DeleteAsync(context);

		// Assert that result indicates success
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Success));
		// Assert that delete was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.DeleteAsync(Arg.Is<TodoList>(x => x.Id == todoList.Id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		string todoListId = "ID";
		TodoListContext context = new(todoListId, TestUserId);
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoListId)
			.ReturnsNull();

		// Act
		var result = await _service.DeleteAsync(context);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.NotFound));
	}

	[Test]
	public async Task DeleteAsync_UserIsNotOwner_ReturnsForbidden()
	{
		// Arrange
		var todoList = TestModels.CreateTestTodoList("invalid user ID");
		TodoListContext context = new(todoList.Id, TestUserId);
		WebApplicationFactory.TodoListsRepository
			.GetByIdAsync(todoList.Id)
			.Returns(todoList);

		// Act
		var result = await _service.DeleteAsync(context);

		// Assert
		Assert.That(result, Is.EqualTo(ServiceResponseStatus.Forbidden));
		// Assert that delete was not called
		await WebApplicationFactory.TodoListsRepository
			.DidNotReceive()
			.DeleteAsync(Arg.Any<TodoList>());
	}
}
