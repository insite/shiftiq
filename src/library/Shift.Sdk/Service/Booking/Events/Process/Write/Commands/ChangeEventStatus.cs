using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeEventStatus : Command
    {
        public string RequestStatus { get; set; }
        public string ScheduleStatus { get; set; }

        public ChangeEventStatus(Guid aggregate, string request, string schedule)
        {
            AggregateIdentifier = aggregate;
            RequestStatus = request;
            ScheduleStatus = schedule;
        }
    }
}
