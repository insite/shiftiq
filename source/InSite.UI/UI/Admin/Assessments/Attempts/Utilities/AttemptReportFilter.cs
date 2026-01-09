using System;

using InSite.Application.Attempts.Read;

namespace InSite.Admin.Assessments.Attempts.Utilities
{
    [Serializable]
    public class AttemptReportFilter : QAttemptFilter
    {
        public bool IncludePendingAttempts { get; set; }
    }
}