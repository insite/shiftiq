using System;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    internal class SummaryTablesNeedDataSourceArgs : EventArgs
    {
        public SummaryTablesDataSource DataSource { get; set; }
    }
}