using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class DeleteProgress : Command
    {
        public DeleteProgress (Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
