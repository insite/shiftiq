using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class PublishProgress : Command
    {
        public PublishProgress (Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
