using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

[TestFixture]
public class UnitOfWorkTests : DataAccessFixture
{
	private IUnitOfWork _unitOfWork;

	[SetUp]
	public void SetUpUnitOfWork()
	{
		_unitOfWork = ServiceScope.ServiceProvider.GetService<IUnitOfWork>()!;
	}

	[Test]
	public async Task Commit_SavesChanges()
	{
		// Arrange
		TodoList todoList = TestModels.CreateTestTodoList();

		// Act
		await _unitOfWork.BeginTransactionAsync();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		await _unitOfWork.CommitAsync();

		// Assert
		Assert.That(DbContext.TodoLists.Any(x => x.Id == todoList.Id));
	}

	[Test]
	public async Task Rollback_DoesNotSaveChanges()
	{
		// Arrange
		TodoList todoList = TestModels.CreateTestTodoList();

		// Act
		await _unitOfWork.BeginTransactionAsync();
		DbContext.Add(todoList);
		await DbContext.SaveChangesAsync();
		await _unitOfWork.RollbackAsync();

		// Assert
		Assert.That(DbContext.TodoLists.Any(x => x.Id == todoList.Id), Is.False);
	}
}
