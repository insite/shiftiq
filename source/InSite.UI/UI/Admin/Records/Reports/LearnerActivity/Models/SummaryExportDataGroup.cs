using Shift.Common;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    public class SummaryExportDataGroup
    {
        public string Heading { get; set; }
        public Counter[] Counters { get; set; }
    }
}