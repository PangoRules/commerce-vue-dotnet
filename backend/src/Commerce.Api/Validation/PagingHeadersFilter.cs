using Commerce.Api.Extensions;
using Commerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Commerce.Api.Validation;

public sealed class PagingHeadersFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // Case 1: action returns Ok(pagedResult)
        IPagedResult? paged = (context.Result as ObjectResult)?.Value as IPagedResult;

        // Case 2: action returns Ok(items) but stores paging in HttpContext.Items
        paged ??= context.HttpContext.GetPaging();

        if (paged is not null)
        {
            var headers = context.HttpContext.Response.Headers;
            headers["X-Total-Count"] = paged.TotalCount.ToString();
            headers["X-Page"] = paged.Page.ToString();
            headers["X-Page-Size"] = paged.PageSize.ToString();
            headers["X-Skip"] = paged.Skip.ToString();
            headers["X-Take"] = paged.Take.ToString();
        }

        await next();
    }
}
