using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VExamEventScheduleFilter : Filter
    {
        public string SortByColumn { get; set; }

        public Guid OrganizationIdentifier { get; set; }
        public DateTimeOffset? ScheduledSince { get; set; }
        public DateTimeOffset? ScheduledBefore { get; set; }
    }
}
