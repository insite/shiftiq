namespace Shift.Api;

public static class HttpResponseExtensions
{
    public static void AddPagination(this HttpResponse response, QueryPagination pagination)
    {
        response.Headers.Append(QueryPagination.HeaderKey, System.Text.Json.JsonSerializer.Serialize(pagination));
    }

    public static void AddPagination(this HttpResponse response, QueryFilter filter, int totalCount)
    {
        AddPagination(response, new QueryPagination(filter.Page, filter.PageSize, totalCount));
    }

    public static void AddPagination(this HttpResponse response, int page, int pageSize, int totalCount)
    {
        AddPagination(response, new QueryPagination(page, pageSize, totalCount));
    }
}