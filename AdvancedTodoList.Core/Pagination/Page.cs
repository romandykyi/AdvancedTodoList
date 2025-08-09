namespace AdvancedTodoList.Core.Pagination;

/// <summary>
/// Represents a paginated collection of items.
/// </summary>
/// <param name="items">The collection of items in the current page.</param>
/// <param name="pageNumber">The page number.</param>
/// <param name="pageSize">The size of the page.</param>
/// <param name="totalCount">The total count of items across all pages.</param>
/// <typeparam name="T">The type of items contained in the page.</typeparam>
public class Page<T>(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
{
    /// <summary>
    /// The collection of items in the current page.
    /// </summary>
    public IEnumerable<T> Items { get; } = items;
    /// <summary>
    /// The page number.
    /// </summary>
    public int PageNumber { get; } = pageNumber;
    /// <summary>
    /// The size of the page (number of items per page).
    /// </summary>
    public int PageSize { get; } = pageSize;
    /// <summary>
    /// The total count of items across all pages.
    /// </summary>
    public int TotalCount { get; } = totalCount;
}
