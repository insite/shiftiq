using System;

using InSite.Application.Attempts.Read;

namespace InSite.Admin.Assessments.Attempts.Utilities
{
    [Serializable]
    public class AdHocAttemptFilter : QAttemptFilter
    {
        public bool IncludeOnlyFirstAttempt { get; set; }
    }
}