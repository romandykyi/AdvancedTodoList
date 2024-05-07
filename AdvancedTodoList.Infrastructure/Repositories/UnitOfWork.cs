using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a unit of work abstraction for managing transactions.
/// </summary>
public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
	private readonly ApplicationDbContext _dbContext = dbContext;
	private IDbContextTransaction? _transaction;

	/// <summary>
	/// Begins a new transaction asynchronously.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <exception cref="InvalidOperationException">Transaction has already begun.</exception>
	public async Task BeginTransactionAsync()
	{
		if (_transaction != null)
			throw new InvalidOperationException("Multiple transactions are not supported.");

		_transaction = await _dbContext.Database.BeginTransactionAsync();
	}

	/// <summary>
	/// Commits the transaction asynchronously.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	public async Task CommitAsync()
	{
		if (_transaction == null)
			throw new InvalidOperationException("Transaction has not begun.");

		await _transaction.CommitAsync();
		_transaction = null;
	}

	/// <summary>
	/// Rolls back the transaction asynchronously.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	public async Task RollbackAsync()
	{
		if (_transaction == null)
			throw new InvalidOperationException("Transaction has not begun.");

		await _transaction.RollbackAsync();
		_transaction = null;
	}
}
