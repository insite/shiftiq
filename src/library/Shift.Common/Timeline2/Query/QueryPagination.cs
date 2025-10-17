using System;

namespace Shift.Common
{
    [Serializable]
    public class QueryPagination
    {
        public const string HeaderKey = "X-Query-Pagination";

        public QueryPagination()
        {

        }

        public QueryPagination(int page, int pageSize, int totalCount)
            : this(page, pageSize)
        {
            TotalCount = totalCount;
        }

        public QueryPagination(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? TotalCount { get; set; }
    }
}