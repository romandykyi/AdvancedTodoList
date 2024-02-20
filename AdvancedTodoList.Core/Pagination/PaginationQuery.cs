namespace EUniversity.Core.Pagination;

/// <summary>
/// Represents the pagination parameters.
/// </summary>
/// <param name="Page">The page number. Default is 1.</param>
/// <param name="PageSize">The number of items per page. Default is 20.</param>
public record PaginationParameters(int Page = 1, int PageSize = 20)
{
	/// <summary>
	/// Minimum size of the page.
	/// </summary>
	public const int MinPageSize = 1;
	/// <summary>
	/// Maximum size of the page.
	/// </summary>
	public const int MaxPageSize = 100;
}
