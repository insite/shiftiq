using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

namespace Shift.Api;

public static class HttpResponseExtensions
{
    public static void AddFingerprint(this HttpResponse response, object data)
    {
        var serialized = JsonConvert.SerializeObject(data);

        var encoded = Encoding.UTF8.GetBytes(serialized);

        var hashed = SHA256.HashData(encoded);

        var hexed = Convert.ToHexString(hashed);

        response.Headers.ETag = hexed;
    }

    public static void AddPagination(this HttpResponse response, QueryPagination pagination)
    {
        if (pagination.Page > 0)
        {
            response.Headers.Append(QueryPagination.HeaderKey, System.Text.Json.JsonSerializer.Serialize(pagination));
        }
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