using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class PeriodCreated : Change
    {
        public PeriodCreated(Guid tenant, string name, DateTimeOffset start, DateTimeOffset end)
        {
            Tenant = tenant;
            Name = name;
            Start = start;
            End = end;
        }

        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}