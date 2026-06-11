using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Periods.Write
{
    public class RenamePeriod : Command
    {
        public RenamePeriod(Guid period, string name)
        {
            AggregateIdentifier = period;
            Name = name;
        }

        public string Name { get; }
    }
}