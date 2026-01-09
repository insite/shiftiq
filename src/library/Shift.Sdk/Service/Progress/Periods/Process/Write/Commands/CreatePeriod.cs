using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Periods.Write
{
    public class CreatePeriod : Command
    {
        public CreatePeriod(Guid period, Guid tenant, string name, DateTimeOffset start, DateTimeOffset end)
        {
            AggregateIdentifier = period;
            Tenant = tenant;
            Name = name;
            Start = start;
            End = end;
        }

        public Guid Tenant { get; }
        public string Name { get; }
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
    }
}