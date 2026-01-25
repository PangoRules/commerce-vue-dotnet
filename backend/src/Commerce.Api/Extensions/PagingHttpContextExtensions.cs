using Commerce.Shared.Responses;

namespace Commerce.Api.Extensions;

public static class PagingHttpContextExtensions
{
    private const string Key = "Paging";

    public static void SetPaging(this HttpContext ctx, IPagedResult paged)
        => ctx.Items[Key] = paged;

    public static IPagedResult? GetPaging(this HttpContext ctx)
        => ctx.Items.TryGetValue(Key, out var value) ? value as IPagedResult : null;
}
