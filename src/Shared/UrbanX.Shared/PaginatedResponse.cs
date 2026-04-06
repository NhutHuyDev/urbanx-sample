namespace UrbanX.Shared;

/// <summary>
/// A generic paginated response returned by list endpoints across all UrbanX services.
/// </summary>
/// <typeparam name="T">The type of the items in the page.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>The items on the current page.</summary>
    public IReadOnlyList<T> Items { get; init; }

    /// <summary>Total number of items matching the query (across all pages).</summary>
    public int Total { get; init; }

    /// <summary>Current 1-based page number.</summary>
    public int Page { get; init; }

    /// <summary>Maximum number of items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of pages, derived from <see cref="Total"/> and <see cref="PageSize"/>.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;

    /// <summary>Whether there is a page before the current one.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Whether there is a page after the current one.</summary>
    public bool HasNextPage => Page < TotalPages;

    public PaginatedResponse(IReadOnlyList<T> items, int total, int page, int pageSize)
    {
        Items = items;
        Total = total;
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// Creates a <see cref="PaginatedResponse{T}"/> from a source list by applying
    /// in-memory skip/take. Use the EF Core overload for database queries.
    /// </summary>
    public static PaginatedResponse<T> Create(IEnumerable<T> source, int page, int pageSize)
    {
        var list = source.ToList();
        var total = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedResponse<T>(items, total, page, pageSize);
    }
}
