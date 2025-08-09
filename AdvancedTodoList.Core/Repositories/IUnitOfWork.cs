namespace AdvancedTodoList.Core.Repositories;

/// <summary>
/// Represents an interface for a unit of work abstraction for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitAsync();

    /// <summary>
    /// Rolls back the transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackAsync();
}