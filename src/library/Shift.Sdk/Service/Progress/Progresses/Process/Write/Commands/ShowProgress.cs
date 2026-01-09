using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class ShowProgress : Command
    {
        public ShowProgress(Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
