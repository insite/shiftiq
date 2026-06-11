using System;

using Shift.Common;

namespace InSite.Application.Registrations.Read
{
    [Serializable]
    public class VAttendanceFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public string LearnerName { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerCode { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
