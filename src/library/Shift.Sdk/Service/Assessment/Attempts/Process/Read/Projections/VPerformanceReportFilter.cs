using System;

namespace InSite.Application.Attempts.Read
{
    [Serializable]
    public class VPerformanceReportFilter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid? FormIdentifier { get; set; }
        public DateTimeOffset? AttemptGradedSince { get; set; }
        public DateTimeOffset? AttemptGradedBefore { get; set; }
        public Guid[] AttemptIds { get; set; }
    }
}
