using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class StartProgress : Command
    {
        public StartProgress(Guid progress, DateTimeOffset started)
        {
            AggregateIdentifier = progress;
            Started = started;
        }

        public DateTimeOffset Started { get; set; }
    }
}
