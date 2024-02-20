using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.IntegrationTests.Setup;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Services.Repositories;

#pragma warning disable IDE0060 // Remove unused parameter
public class RepositoryTests : IntegrationTest
{
	private record SimpleDto<TKey>(TKey Id);

	// Test cases which provide a test model creator and invalid IDs
	public static object[] DefaultTestCases => new object[]
	{
		new object[] { () => TestModels.CreateTestTodoList(), "" },
		new object[] { () =>TestModels.CreateTestTodoItem(DbPopulator.ExistingTodoListId), -1 }
	};

	private async Task<TEntity> CreateTestEntityAsync<TEntity, TKey>(Func<TEntity> createEntity)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		TEntity entity = createEntity();
		DbContext.Set<TEntity>().Add(entity);
		await DbContext.SaveChangesAsync();
		DbContext.ChangeTracker.Clear();

		return entity;
	}

	[Test]
	[TestCaseSource(nameof(DefaultTestCases))]
	// sampleKey argument here is needed for NUnit to recognize type of TKey
	public async Task AddAsync_AddsEntityToDb<TEntity, TKey>(Func<TEntity> createEntity, TKey sampleKey)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;
		var entity = createEntity();

		// Act
		await repository.AddAsync(entity);

		// Assert that entity was added
		var foundEntity = await DbContext.Set<TEntity>()
			.Where(x => x.Id.Equals(entity.Id))
			.FirstOrDefaultAsync();
		Assert.That(foundEntity, Is.Not.Null);
	}

	[Test]
	[TestCaseSource(nameof(DefaultTestCases))]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull<TEntity, TKey>(Func<TEntity> createEntity, TKey id)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;

		// Act
		var result = await repository.GetByIdAsync(id);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	[TestCaseSource(nameof(DefaultTestCases))]
	public async Task GetByIdAsync_EntityExists_ReturnsEntityWithValidId<TEntity, TKey>(Func<TEntity> createEntity, TKey id)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;
		var entity = await CreateTestEntityAsync<TEntity, TKey>(createEntity);

		// Act
		var result = await repository.GetByIdAsync(entity.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(entity.Id));
	}

	[Test]
	[TestCaseSource(nameof(DefaultTestCases))]
	public async Task GetByIdAndMapAsync_EntityDoesNotExist_ReturnsNull<TEntity, TKey>(Func<TEntity> createEntity, TKey id)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;

		// Act
		var result = await repository.GetByIdAndMapAsync<SimpleDto<TKey>>(id);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	[TestCaseSource(nameof(DefaultTestCases))]
	public async Task GetByIdAndMapAsync_EntityExists_ReturnsEntityWithValidId<TEntity, TKey>(Func<TEntity> createEntity, TKey id)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;
		var entity = await CreateTestEntityAsync<TEntity, TKey>(createEntity);

		// Act
		var dto = await repository.GetByIdAndMapAsync<SimpleDto<TKey>>(entity.Id);

		// Assert
		Assert.That(dto, Is.Not.Null);
		Assert.That(dto.Id, Is.EqualTo(entity.Id));
	}

	public static object[] UpdateTestCases => new object[]
	{
		new object[] { () => TestModels.CreateTestTodoList(),
			"", (TodoList list) => { list.Name = "New"; }, (TodoList list) => list.Name == "New" },
		new object[] { () => TestModels.CreateTestTodoItem(DbPopulator.ExistingTodoListId),
			-1, (TodoItem item) => { item.Name = "New"; }, (TodoItem item) => item.Name == "New" }
	};
	[Test]
	[TestCaseSource(nameof(UpdateTestCases))]
	public async Task UpdateAsync_UpdatesEntity<TEntity, TKey>(Func<TEntity> createEntity, TKey id,
		Action<TEntity> update, Func<TEntity, bool> assertUpdated)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;
		var entity = await CreateTestEntityAsync<TEntity, TKey>(createEntity);

		// Act
		update(entity);
		await repository.UpdateAsync(entity);

		// Assert
		var actualEntity = await DbContext.Set<TEntity>()
			.Where(x => x.Id.Equals(entity.Id))
			.FirstAsync();
		Assert.That(assertUpdated(actualEntity), "Entity was not updated.");
	}

	[Test]
	[TestCaseSource(nameof(DefaultTestCases))]
	public async Task DeleteAsync_DeletesEntity<TEntity, TKey>(Func<TEntity> createEntity, TKey id)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		// Arrange
		var repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;
		var entity = await CreateTestEntityAsync<TEntity, TKey>(createEntity);

		// Act
		await repository.DeleteAsync(entity);

		// Assert
		Assert.That(DbContext.Set<TEntity>().Any(x => x.Id.Equals(entity.Id)), Is.False, "Entity was not deleted.");
	}
}
#pragma warning restore IDE0060 // Remove unused parameter
