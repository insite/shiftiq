using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QEnrollmentFilter : Filter
    {
        public Guid GradebookIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
        public string SearchKeyword { get; set; }
        public bool? IsPeriodAssigned { get; set; }
        public string LearnerFullName { get; set; }
        public DateTimeOffset? GradedSince { get; set; }
        public DateTimeOffset? GradedBefore { get; set; }
    }
}
