using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class LockProgress : Command
    {
        public LockProgress(Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
