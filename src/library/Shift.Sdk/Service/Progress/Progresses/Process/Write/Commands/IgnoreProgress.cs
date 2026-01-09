using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class IgnoreProgress : Command
    {
        public bool IsIgnored { get; }

        public IgnoreProgress(Guid progress, bool isIgnored)
        {
            AggregateIdentifier = progress;
            IsIgnored = isIgnored;
        }
    }
}
