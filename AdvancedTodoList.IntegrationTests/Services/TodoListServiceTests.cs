using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListServiceTests : IntegrationTest
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
		// Arrange: add test list to the DB
		TodoList testList = new()
		{
			Name = "Test list",
			Description = ""
		};
		DbContext.TodoLists.Add(testList);
		await DbContext.SaveChangesAsync();

		// Act: try to obtain a test list's DTO by its ID
		var result = await _service.GetByIdAsync(testList.Id);

		// Assert that returned DTO matches
		Assert.That(result, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(result.Id, Is.EqualTo(testList.Id));
			Assert.That(result.Name, Is.EqualTo(testList.Name));
		});
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Act: try to obtain a test list with non existent ID
		var result = await _service.GetByIdAsync("_");

		// Assert that null is returned
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CreateAsync_AddsEntityToDb()
	{
		// Arrange: initialize a DTO
		TodoListCreateDto dto = new("Test entity", "...");

		// Act: call the method
		var result = await _service.CreateAsync(dto);

		// Assert that entity was added to the DB
		var foundEntity = await DbContext.TodoLists
			.AsNoTracking()
			.Where(x => x.Id == result.Id)
			.SingleOrDefaultAsync();
		Assert.That(foundEntity, Is.Not.Null);
	}
}
