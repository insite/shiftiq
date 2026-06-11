using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Periods.Write
{
    public class DeletePeriod : Command
    {
        public DeletePeriod(Guid period)
        {
            AggregateIdentifier = period;
        }
    }
}