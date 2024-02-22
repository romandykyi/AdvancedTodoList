using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services.Repositories;
using AdvancedTodoList.Core.Specifications;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace AdvancedTodoList.IntegrationTests.Services.Repositories;

/// <summary>
/// Represents an abstract test fixture for testing repositories.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of entity's primary key.</typeparam>
public abstract class BaseRepositoryTests<TEntity, TKey> : IntegrationTest
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	protected class TestSpecification : ISpecification<TEntity>
	{
		public Expression<Func<TEntity, bool>> Criteria { get; set; } = _ => true;

		public List<Expression<Func<TEntity, object>>> Includes { get; set; } = [];

		public List<string> IncludeStrings { get; set; } = [];
	}

	/// <summary>
	/// When implemented, gets an ID of non-existing entity.
	/// </summary>
	protected abstract TKey NonExistingId { get; }

	protected IRepository<TEntity, TKey> Repository { get; private set; }

	[SetUp]
	public void SetUpRepository()
	{
		Repository = ServiceScope.ServiceProvider.GetService<IRepository<TEntity, TKey>>()!;
	}

	/// <summary>
	/// When implemented, creates a test entity which can be added to the database.
	/// </summary>
	/// <returns>
	/// A test entity which can be added to the database.
	/// </returns>
	protected abstract Task<TEntity> CreateTestEntityAsync();

	/// <summary>
	/// When implemented, performs a test update on an entity.
	/// </summary>
	/// <param name="entity">Entity to be updated.</param>
	protected abstract void UpdateEntity(TEntity entity);

	/// <summary>
	/// When implemented, asserts that entity was updated.
	/// </summary>
	/// <param name="updatedEntity">Entity that should've been updated.</param>
	protected abstract void AssertUpdated(TEntity updatedEntity);

	/// <summary>
	/// Adds a test entity to the database and untracks it.
	/// </summary>
	/// <returns>
	/// A test entity which was added to the database.
	/// </returns>
	protected async Task<TEntity> AddTestEntityToDbAsync()
	{
		TEntity entity = await CreateTestEntityAsync();
		DbContext.Set<TEntity>().Add(entity);
		await DbContext.SaveChangesAsync();
		DbContext.ChangeTracker.Clear();

		return entity;
	}

	[Test]
	public async Task AddAsync_AddsEntityToDb()
	{
		// Arrange
		var entity = await CreateTestEntityAsync();

		// Act
		await Repository.AddAsync(entity);

		// Assert that entity was added
		var foundEntity = await DbContext.Set<TEntity>()
			.Where(x => x.Id.Equals(entity.Id))
			.FirstOrDefaultAsync();
		Assert.That(foundEntity, Is.Not.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
	{
		// Act
		var result = await Repository.GetByIdAsync(NonExistingId);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetByIdAsync_EntityExists_ReturnsEntityWithValidId()
	{
		// Arrange
		var entity = await AddTestEntityToDbAsync();

		// Act
		var result = await Repository.GetByIdAsync(entity.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(entity.Id));
	}

	[Test]
	public async Task GetPageAsync_AppliesSpecification()
	{
		// Arrange
		var entity = await AddTestEntityToDbAsync();
		TestSpecification specification = new()
		{
			Criteria = x => x.Id.Equals(entity.Id)
		};

		// Act
		var page = await Repository.GetPageAsync<TEntity>(new(1, 5), specification);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(page.TotalCount, Is.EqualTo(1));
			Assert.That(page.Items.Single().Id, Is.EqualTo(entity.Id));
		});
	}

	[Test]
	public async Task GetPageAsync_AppliesPaginationParameters()
	{
		// Arrange
		var entity = await CreateTestEntityAsync();
		TestSpecification specification = new();
		PaginationParameters paginationParameters = new(2, 10);

		// Act
		var page = await Repository.GetPageAsync<TEntity>(paginationParameters, specification);

		// Assert
		int totalCount = await DbContext.Set<TEntity>().CountAsync();
		Assert.Multiple(() =>
		{
			Assert.That(page.TotalCount, Is.EqualTo(totalCount));
			Assert.That(page.PageNumber, Is.EqualTo(paginationParameters.Page));
			Assert.That(page.PageSize, Is.EqualTo(paginationParameters.PageSize));
		});
	}

	[Test]
	public async Task UpdateAsync_UpdatesEntity()
	{
		// Arrange
		var entity = await AddTestEntityToDbAsync();

		// Act
		UpdateEntity(entity);
		await Repository.UpdateAsync(entity);

		// Assert
		var updatedEntity = await DbContext.Set<TEntity>()
			.Where(x => x.Id.Equals(entity.Id))
			.FirstAsync();
		AssertUpdated(updatedEntity);
	}

	[Test]
	public async Task DeleteAsync_DeletesEntity()
	{
		// Arrange
		var entity = await AddTestEntityToDbAsync();

		// Act
		await Repository.DeleteAsync(entity);

		// Assert
		Assert.That(DbContext.Set<TEntity>().Any(x => x.Id.Equals(entity.Id)), Is.False, "Entity was not deleted.");
	}
}
