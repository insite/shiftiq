using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VReportFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ReportTitle { get; set; }
        public string ReportDescription { get; set; }
        public string[] ReportTypes { get; set; }
        public bool? IsCreator { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTimeOffset? ModifiedSince { get; set; }
        public DateTimeOffset? ModifiedBefore { get; set; }

        public bool IncludeShared { get; set; }
    }
}
