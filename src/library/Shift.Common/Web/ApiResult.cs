using System.Net;
using System.Net.Http.Headers;

using Shift.Common;

namespace Shift.Common
{
    public class ApiResult
    {
        public bool IsOK() => Status == HttpStatusCode.OK;

        public HttpStatusCode Status { get; set; }

        public Problem Problem { get; set; }

        public HttpResponseHeaders Headers { get; set; }

        public QueryPagination Pagination { get; set; }

        public ApiResult(HttpStatusCode status, HttpResponseHeaders headers)
        {
            Status = status;
            Headers = headers;
        }
    }

    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; } = default;

        public ApiResult(HttpStatusCode status, HttpResponseHeaders headers)
            : base(status, headers)
        {

        }
    }
}