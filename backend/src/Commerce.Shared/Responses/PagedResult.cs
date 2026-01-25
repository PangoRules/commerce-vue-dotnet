namespace Commerce.Shared.Responses;

public interface IPagedResult
{
    int Page { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int Skip { get; }
    int Take { get; }
}

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount
) : IPagedResult
{
    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
}
