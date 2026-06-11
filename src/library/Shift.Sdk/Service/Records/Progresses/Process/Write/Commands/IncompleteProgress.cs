using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class IncompleteProgress : Command
    {
        public IncompleteProgress(Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
