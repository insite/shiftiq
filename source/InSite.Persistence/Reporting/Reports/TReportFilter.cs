using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TReportFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ReportTitle { get; set; }
        public string ReportDescription { get; set; }
        public string ReportPrivacyScope { get; set; }
        public bool? IsCreator { get; set; }

        public DateTimeOffset? ModifiedSince { get; set; }
        public DateTimeOffset? ModifiedBefore { get; set; }
    }
}
