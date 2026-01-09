using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class LimitExamTime : Command
    {
        public LimitExamTime(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
