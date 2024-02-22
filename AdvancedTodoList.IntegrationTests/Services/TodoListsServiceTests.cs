﻿using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ReturnsExtensions;

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
		TodoListCreateDto dto = new("Test entity", "...");
		WebApplicationFactory.TodoListsRepository
			.AddAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act: call the method
		var result = await _service.CreateAsync(dto);

		// Assert that method was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.AddAsync(Arg.Is<TodoList>(x => x.Name == dto.Name && x.Description == dto.Description));
	}

	[Test]
	public async Task EditAsync_EntityExists_Succeeds()
	{
		// Arrange
		string id = "ID";
		TodoListCreateDto updateDto = new("Name", "Description");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(id)
			.Returns(true);
		WebApplicationFactory.TodoListsRepository
			.UpdateAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.EditAsync(id, updateDto);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that update was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.UpdateAsync(Arg.Is<TodoList>(x => x.Id == id &&
				x.Name == updateDto.Name && 
				x.Description == updateDto.Description));
	}

	[Test]
	public async Task EditAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string id = "ID";
		TodoListCreateDto updateDto = new("Name", "Description");
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(id)
			.Returns(false);

		// Act
		bool result = await _service.EditAsync(id, updateDto);

		// Assert that result is false
		Assert.That(result, Is.False);
	}

	[Test]
	public async Task DeleteAsync_EntityExists_Succeeds()
	{
		// Arrange
		string id = "ID";
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(id)
			.Returns(true);
		WebApplicationFactory.TodoListsRepository
			.DeleteAsync(Arg.Any<TodoList>())
			.Returns(Task.FromResult);

		// Act
		bool result = await _service.DeleteAsync(id);

		// Assert that result is true
		Assert.That(result, Is.True);
		// Assert that delete was called
		await WebApplicationFactory.TodoListsRepository
			.Received()
			.DeleteAsync(Arg.Is<TodoList>(x => x.Id == id));
	}

	[Test]
	public async Task DeleteAsync_EntityDoesNotExist_ReturnsFalse()
	{
		// Arrange
		string id = "ID";
		WebApplicationFactory.EntityExistenceChecker
			.ExistsAsync<TodoList, string>(id)
			.Returns(false);

		// Act
		bool result = await _service.DeleteAsync(id);

		// Assert that result is false
		Assert.That(result, Is.False);
	}
}
