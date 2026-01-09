using System.Collections.Generic;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    internal class SummaryTablesDataSource
    {
        public IList<SearchResultDataItem> Items { get; }
        public bool IsSummaryCountStrategy { get; }

        public SummaryTablesDataSource(IList<SearchResultDataItem> items, bool isSummaryCountStrategy)
        {
            Items = items;
            IsSummaryCountStrategy = isSummaryCountStrategy;
        }
    }
}