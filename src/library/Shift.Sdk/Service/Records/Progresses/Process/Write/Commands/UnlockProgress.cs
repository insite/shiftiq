using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class UnlockProgress : Command
    {
        public UnlockProgress(Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
