using System;

namespace Shift.Sdk.UI
{
    public class AttendanceCriteria
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}