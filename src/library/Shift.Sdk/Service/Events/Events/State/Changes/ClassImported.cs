using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ClassImported : Change
    {
        public Guid Tenant { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public int? DurationQuantity { get; set; }
        public string DurationUnit { get; set; }

        public ClassImported(Guid tenant, string title, DateTimeOffset start, DateTimeOffset end, int? durationQuantity, string durationUnit, string source)
        {
            Tenant = tenant;
            Title = title;
            Source = source;

            Start = start;
            End = end;
            
            DurationQuantity = durationQuantity;
            DurationUnit = durationUnit;
        }
    }
}